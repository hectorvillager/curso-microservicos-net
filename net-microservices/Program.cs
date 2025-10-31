using webapi.common.dependencyinjection;
using webapi.common;
using FluentValidation;
using webapi.infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using webapi.common.openapi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using webapi.common.configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

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

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' [espacio] y luego tu token JWT"
    });

    options.OperationFilter<GlobalErrorResponsesOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Type = context.ProblemDetails.Type ?? "about:blank";
        context.ProblemDetails.Instance = context.HttpContext.Request.Path;
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
    };
});

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings not configured");

builder.Services.AddSingleton(jwtSettings);

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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ClockSkew = TimeSpan.Zero
    };
});

// NO agregar FallbackPolicy aquí
builder.Services.AddAuthorization();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddInjectables();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("PermitirTodo");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Protege todos los endpoints EXCEPTO los que marques como AllowAnonymous
app.MapFeatures();

app.Run();