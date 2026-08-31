using Microsoft.EntityFrameworkCore;
using RestaurantPOS.API.MappingProfiles;
using RestaurantPOS.Core.Interfaces;
using RestaurantPOS.Infrastructure;
using RestaurantPOS.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// DbContext যোগ করা
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(typeof(AppMappingProfile));


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
app.MapControllers();


app.Run();


