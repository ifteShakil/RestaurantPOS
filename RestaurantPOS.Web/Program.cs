using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using RestaurantPOS.Web.Data;
using RestaurantPOS.Web.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
//builder.Services.AddSingleton<WeatherForecastService>();

// ⭐ API-কে কল করার জন্য HttpClient রেজিস্টার
builder.Services.AddHttpClient("RestaurantAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
});

// আমাদের নিজস্ব সার্ভিস, যেটা এই HttpClient ব্যবহার করবে
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<AppState>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
