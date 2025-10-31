using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using webapi.common;
using webapi.common.dependencyinjection;
using webapi.common.security;

namespace webapi.features.auth.commands;

public class Login : IFeatureModule
{
    public record struct Request(
        [Required][property: Required] string Username,
        [Required][property: Required] string Password
    );

    public record struct Response(
        [Required][property: Required] string Token,
        [Required][property: Required] string Username,
        [Required][property: Required] DateTime ExpiresAt
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (IService service, [FromBody] Request request) =>
        {
            var response = await service.Handler(request);
            
            if (response == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(response);
        })        
        .AllowAnonymous() // Permite acceso sin autenticación
        .WithOpenApi()
        .WithName("Login")
        .WithSummary("Autenticar usuario")
        .WithDescription("Endpoint para autenticar un usuario y obtener un token JWT")
        .WithTags("Autenticación")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    public interface IService
    {
        Task<Response?> Handler(Request request);
    }

    [Injectable]
    public class Service : IService
    {
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly int _expirationMinutes = 60; // Puedes inyectar esto desde configuración

        public Service(IJwtTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        public async Task<Response?> Handler(Request request)
        {
            // Validar credenciales (DEMO - En producción usar hash y base de datos)
            if (!await ValidateCredentials(request.Username, request.Password))
            {
                return null; // Credenciales inválidas
            }

            // Generar token
            var userId = Guid.NewGuid().ToString(); // En producción, obtener de BD
            var token = _tokenGenerator.GenerateToken(request.Username, userId);

            var response = new Response(
                Token: token,
                Username: request.Username,
                ExpiresAt: DateTime.UtcNow.AddMinutes(_expirationMinutes)
            );

            return response;
        }

        private Task<bool> ValidateCredentials(string username, string password)
        {
            // DEMO: Credenciales hardcodeadas
            // En producción: consultar BD y verificar hash con BCrypt o similar
            var isValid = username == "admin" && password == "admin123";
            return Task.FromResult(isValid);
        }
    }
}