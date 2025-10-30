using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using Microsoft.AspNetCore.Mvc;

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
        app.MapGet("/ingredientes/{id:guid}", async (
            IService service,
            Guid id,
            [FromHeader(Name = "x-dni")] string? dni) =>
        {
            var response = await service.Handler(id);
            return Results.Ok(response);
        })
        .WithOpenApi()
        .WithName("GetIngredient")
        .WithSummary("Obtener un ingrediente por ID")
        .WithDescription("Endpoint para obtener los detalles de un ingrediente específico")
        .WithTags("Ingredientes")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public interface IService
    {
        Task<Response> Handler(Guid id);
    }

    [Injectable]
    public class Service(IGet<Ingredient, Guid> repository) : IService
    {
        private readonly IGet<Ingredient, Guid> _repository = repository;

        public async Task<Response> Handler(Guid id)
        {
            var ingredient = await _repository.GetAsync(id);

            var response = new Response(ingredient.Id, ingredient.Name, ingredient.Cost);

            return response;
        }
    }

    [Injectable]
    public class Repository(ApplicationDbContext context) : IGet<Ingredient, Guid>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Ingredient> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GetOrThrowAsync<Ingredient, Guid>(id, cancellationToken, false);
        }
    }
}