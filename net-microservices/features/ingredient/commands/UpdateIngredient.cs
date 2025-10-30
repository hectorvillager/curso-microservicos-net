using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
using webapi.infrastructure;

namespace webapi.features.ingredient.commands;

public class UpdateIngredient : IFeatureModule
{
    public record struct Request(
        [Required][property: Required] string Name,
        [Required][property: Required] decimal Cost
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/ingredientes/{id:guid}", async (IService service, Guid id, [FromBody] Request request) =>
        {
            await service.Handler(id, request);
            return Results.NoContent();
        })        
        .WithOpenApi()
        .WithName("UpdateIngredient")
        .WithSummary("Actualizar un ingrediente existente")
        .WithDescription("Endpoint para actualizar el nombre y costo de un ingrediente")
        .WithTags("Ingredientes")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public interface IService
    {
        Task Handler(Guid id, Request request);
    }

    [Injectable]
    public class Service(IUpdate<Ingredient, Guid> repository) : IService
    {
        private readonly IUpdate<Ingredient, Guid> _repository = repository;

        public async Task Handler(Guid id, Request request)
        {
            var ingredient = await _repository.GetAsync(id);
            
            ingredient.Update(request.Name, request.Cost);
            
            _repository.UpdateAsync(id, ingredient);
        }
    }
    
    [Injectable]
    public class Repository(ApplicationDbContext context) : IUpdate<Ingredient, Guid>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Ingredient> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GetOrThrowAsync<Ingredient, Guid>(id, cancellationToken);
        }

        public void UpdateAsync(Guid id, Ingredient entity, CancellationToken cancellationToken = default)
        {
            _context.Update(entity);
            _context.SaveChangesAsync(cancellationToken);
        }
    }
}