using Enhanzer_API.Authentication.Endpoints;
using Enhanzer_API.Authentication.Services;
using Enhanzer_API.Locations.Endpoints;
using Enhanzer_API.Locations.Persistence;
using Enhanzer_API.Locations.Services;
using Enhanzer_API.Middleware;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddValidation();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "http://192.168.213.1:8080/")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var locationsConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(locationsConnectionString))
{
    throw new InvalidOperationException(
        "The 'ConnectionStrings:DefaultConnection' setting is required to use the locations API.");
}

builder.Services.AddDbContext<LocationsDbContext>(options =>
    options.UseSqlServer(locationsConnectionString));
builder.Services.AddScoped<ILocationsService, LocationsService>();
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>(client =>
{
    client.BaseAddress = new Uri("https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke");
});
builder.Services.AddExceptionHandler<BadHttpRequestExceptionHandler>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.MapAuthEndpoints();
app.MapLocationsEndpoints();

app.Run();

