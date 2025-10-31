using webapi.common.dependencyinjection;
using webapi.common;
using FluentValidation;
using webapi.infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using webapi.common.openapi;
using Microsoft.AspNetCore.Mvc;
using webapi.common.infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type =>
    {
        if (type == typeof(CustomProblemDetails))
            return "CustomProblemDetails";

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

    options.MapType<ProblemDetails>(() => new OpenApiSchema
    {
        Type = "object",
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["type"] = new OpenApiSchema { Type = "string", Example = new Microsoft.OpenApi.Any.OpenApiString("about:blank") },
            ["title"] = new OpenApiSchema { Type = "string" },
            ["status"] = new OpenApiSchema { Type = "integer", Format = "int32" },
            ["detail"] = new OpenApiSchema { Type = "string" },
            ["instance"] = new OpenApiSchema { Type = "string" },
            ["extensions"] = new OpenApiSchema
            {
                Type = "object",
                AdditionalPropertiesAllowed = true,
                Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["traceId"] = new Microsoft.OpenApi.Any.OpenApiString("00-abc123-def456-01"),
                    ["timestamp"] = new Microsoft.OpenApi.Any.OpenApiString("2025-10-30T10:30:00Z")
                }
            }
        },
        AdditionalPropertiesAllowed = false
    });

    options.OperationFilter<GlobalErrorResponsesOperationFilter>();
});

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("PizzaDb");


    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

//builder.Services.AddScoped<IGetOrThrowAsync>(sp => sp.GetRequiredService<ApplicationDbContext>());


builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Type = context.ProblemDetails.Type ?? "about:blank";
        context.ProblemDetails.Instance = context.HttpContext.Request.Path;

        // Añadir información adicional si lo deseas
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddInjectables();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapFeatures();

app.Run();

