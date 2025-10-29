using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.infrastructure;
using webapi.common.dependencyinjection;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.ingredient.queries;

public class GetIngredient : IFeatureModule
{
    public record struct Response(
        [Required][property: Required] Guid Id,
        [Required][property: Required] string Name,
        [Required][property: Required] decimal Cost
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/ingredientes/{id:guid}", async (IService service, Guid id) =>
        {
            var response = await service.Handler(id);
            return response is not null
                ? Results.Ok(response)
                : Results.NotFound();
        })
        .WithOpenApi()
        .WithName("GetIngredient")
        .WithSummary("Obtener un ingrediente por Id")
        .WithDescription("Endpoint para obtener un ingrediente específico por su identificador")
        .WithTags("Ingredientes")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public interface IService
    {
        Task<Response?> Handler(Guid id);
    }

    [Injectable]
    public class Service : IService
    {
        private readonly IRead<Ingredient> _repository;
        public Service(IRead<Ingredient> repository)
        {
            _repository = repository;
        }

        public async Task<Response?> Handler(Guid id)
        {
            var ingredient = await _repository.GetByIdAsync(id);
            return ingredient is not null
                ? new Response(ingredient.Id, ingredient.Name, ingredient.Cost)
                : null;
        }
    }

    [Injectable]
    public class Repository : IRead<Ingredient>
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Ingredient?> GetByIdAsync(Guid id)
        {
            return await _context.Ingredients.FindAsync(id);
        }
    }
}

// filepath: src/webapi/common/infrastructure/IRead.cs
public interface IRead<T>
{
    Task<T?> GetByIdAsync(Guid id);
}