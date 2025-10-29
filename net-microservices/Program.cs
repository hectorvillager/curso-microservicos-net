using webapi.common.dependencyinjection;
using webapi.common;
using FluentValidation;
using webapi.infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
    {
        if (type.DeclaringType != null)
        {

            return $"{type.DeclaringType.Name}.{type.Name}";
        }
        return type.FullName?.Replace("+", ".").Replace(".", "") ?? type.Name;
    });
    
    options.MapType<decimal>(() => new OpenApiSchema 
    { 
        Type = "number", 
        Format = "decimal"
    });
    
    options.MapType<decimal?>(() => new OpenApiSchema 
    { 
        Type = "number", 
        Format = "decimal",
        Nullable = true
    });
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("PizzaDb");
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddInjectables();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapFeatures();



app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
