using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.infrastructure;
using webapi.common.dependencyinjection;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.ingredient.commands;

public class UpdateIngredient : IFeatureModule
{
    public record struct Request(
        [Required][property: Required] Guid Id,
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
        app.MapPut("/ingredientes/{id:guid}", async (IService service, Guid id, [FromBody] Request request) =>
        {
            var response = await service.Handler(id, request);
            return response is not null ? Results.Ok(response) : Results.NotFound();
        })
        .WithOpenApi()
        .WithName("UpdateIngredient")
        .WithSummary("Actualizar un ingrediente por Id")
        .WithDescription("Endpoint para actualizar un ingrediente específico por su identificador")
        .WithTags("Ingredientes")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public interface IService
    {
        Task<Response?> Handler(Guid id, Request request);
    }

    [Injectable]
    public class Service : IService
    {
        private readonly IUpdate<Ingredient> _repository;
        public Service(IUpdate<Ingredient> repository)
        {
            _repository = repository;
        }

        public async Task<Response?> Handler(Guid id, Request request)
        {
            var ingredient = await _repository.UpdateAsync(id, request.Name, request.Cost);
            return ingredient is not null
                ? new Response(ingredient.Id, ingredient.Name, ingredient.Cost)
                : null;
        }
    }

    [Injectable]
    public class Repository : IUpdate<Ingredient>
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ingredient?> UpdateAsync(Guid id, string name, decimal cost)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient is null) return null;
            ingredient.Update(name, cost); // Usa el método de tu entidad para validar y actualizar
            await _context.SaveChangesAsync();
            return ingredient;
        }
    }
}

// filepath: src/webapi/common/infrastructure/IUpdate.cs
public interface IUpdate<T>
{
    Task<T?> UpdateAsync(Guid id, string name, decimal cost);
}