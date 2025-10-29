using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.infrastructure;
using webapi.common.dependencyinjection;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.ingredient.queries;

public class GetIngredients : IFeatureModule
{
    public record struct Response(
        [Required][property: Required] Guid Id,
        [Required][property: Required] string Name,
        [Required][property: Required] decimal Cost
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/ingredientes", async (IService service) =>
        {
            var response = await service.Handler();
            return Results.Ok(response);
        })
        .WithOpenApi()
        .WithName("GetAllIngredients")
        .WithSummary("Obtener todos los ingredientes")
        .WithDescription("Endpoint para obtener la lista de todos los ingredientes")
        .WithTags("Ingredientes")
        .Produces<Response[]>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public interface IService
    {
        Task<Response[]> Handler();
    }

    [Injectable]
    public class Service : IService
    {
        private readonly IReadAll<Ingredient> _repository;
        public Service(IReadAll<Ingredient> repository)
        {
            _repository = repository;
        }

        public async Task<Response[]> Handler()
        {
            var ingredients = await _repository.GetAllAsync();
            return ingredients.Select(i => new Response(i.Id, i.Name, i.Cost)).ToArray();
        }
    }

    [Injectable]
    public class Repository : IReadAll<Ingredient>
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ingredient>> GetAllAsync()
        {
            return await _context.Ingredients.ToListAsync();
        }
    }
}

// filepath: src/webapi/common/infrastructure/IReadAll.cs
public interface IReadAll<T>
{
    Task<IEnumerable<T>> GetAllAsync();
}