using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.API.MappingProfiles;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Core.Models;
using RestaurantPOS.Infrastructure;
using RestaurantPOS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

// Bind JWT Settings
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);

// Add JWT Authentication 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS — Blazor Web
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7189", "http://localhost:5280") // আপনার Blazor Web-এর URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();      // মাইগ্রেশন apply — এতে PaymentMethodType ডাটাও ঢুকে যাবে
    await DataSeeder.SeedAsync(db);        // তারপর Branch ডাটা সিড হবে
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
   
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");   // Before UseAuthentication

app.UseAuthentication();             // Who you are (identity check)
app.UseAuthorization();              // What you can do (access control)
app.MapControllers();


app.Run();


