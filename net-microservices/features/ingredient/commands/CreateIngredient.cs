using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;

namespace webapi.features.ingredient.commands;

public class CreateIngredient : IFeatureModule
{
    public record struct Request(
        [Required][property: Required] string Name,
        [Required][property: Required] decimal Cost
    );
    public record struct Response(
        [Required][property: Required] Guid Id,
        [Required][property: Required] string Name, 
        [Required][property: Required] decimal Cost
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/ingredientes", async (IService service, [FromBody] Request request) =>
        {
            var response = await service.Handler(request);
            return Results.Ok(response);
        })        
        .WithOpenApi()
        .WithName("CreateIngredient")
        .WithSummary("Crear un nuevo ingrediente")
        .WithDescription("Endpoint para crear un nuevo ingrediente con su nombre y costo")
        .WithTags("Ingredientes")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest); // Opcional: para errores
    }

    public interface IService
    {
        Task<Response> Handler(Request request);
    }

    [Injectable]
    public class Service : IService
    {
        public Task<Response> Handler(Request request)
        {
            var response = new Response(
                Guid.NewGuid(),
                request.Name,
                request.Cost
            );
            return Task.FromResult(response);
        }
    }
}