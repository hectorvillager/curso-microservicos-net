using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
using webapi.infrastructure;

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
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public interface IService
    {
        Task<Response> Handler(Request request);
    }

    [Injectable]
    public class Service(IAdd<Ingredient> repository) : IService
    {
        private readonly IAdd<Ingredient> _repository = repository;

        public async Task<Response> Handler(Request request)
        {
            var ingredient = Ingredient.Create(Guid.NewGuid(), request.Name, request.Cost);

            await _repository.AddAsync(ingredient);

            var response = new Response(ingredient.Id, ingredient.Name, ingredient.Cost);
            
            return response;
        }
    }
    
    [Injectable]
    public class Repository(ApplicationDbContext context) : IAdd<Ingredient>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(Ingredient entity, CancellationToken cancellationToken = default)
        {
            await _context.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}