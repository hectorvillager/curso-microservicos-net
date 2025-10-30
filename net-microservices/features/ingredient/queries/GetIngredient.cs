using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.ingredient.queries;

public class GetIngredient : IFeatureModule
{
    public record struct Response(
        [Required][property: Required] Guid Id,
        [Required][property: Required] string Name, 
        [Required][property: Required] decimal Cost
    );

    public record struct IngredientQuery(
        string? Name,
        int Page = 1,
        int Size = 10
    );

    public record struct IngredientListResponse(
        int Total,
        int Page,
        int Size,
        List<GetIngredient.Response> Items
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/ingredientes/{id:guid}", async (IService service, Guid id, HttpContext http) =>
        {
            var response = await service.Handler(id);

            http.Response.Headers["X-Custom-Header"] = "Valor personalizado";

            return Results.Ok(response);
        })
        .WithOpenApi()
        .WithName("GetIngredient")
        .WithSummary("Obtener un ingrediente por ID")
        .WithDescription("Endpoint para obtener los detalles de un ingrediente específico")
        .WithTags("Ingredientes")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
        
        app.MapGet("/ingredientes/query", async (
                                [AsParameters] IngredientQuery query,
                                IService service) =>
        {
            var result = await service.List(query);
            return Results.Ok(result);
        })
        .WithOpenApi()
        .WithName("ListIngredients")
        .WithSummary("Listar ingredientes paginados y filtrados")
        .Produces<IngredientListResponse>(StatusCodes.Status200OK);
    }

    public interface IService
    {
        Task<Response> Handler(Guid id);
        Task<IngredientListResponse> List(IngredientQuery query);
    }

    [Injectable]
    public class Service(IGetQuery<Ingredient, Guid> repository) : IService
    {
        private readonly IGetQuery<Ingredient, Guid> _repository = repository;

        public async Task<Response> Handler(Guid id)
        {
            var ingredient = await _repository.GetAsync(id);

            var response = new Response(ingredient.Id, ingredient.Name, ingredient.Cost);

            return response;
        }
        
        public async Task<IngredientListResponse> List(IngredientQuery query)
        {
            var ingredients = _repository.Query();

            if (!string.IsNullOrWhiteSpace(query.Name))
                ingredients = ingredients.Where(x => x.Name.Contains(query.Name));

            var total = await ingredients.CountAsync();
            var items = await ingredients
                .Skip((query.Page - 1) * query.Size)
                .Take(query.Size)
                .Select(x => new GetIngredient.Response(x.Id, x.Name, x.Cost))
                .ToListAsync();

            return new IngredientListResponse(total, query.Page, query.Size, items);
        }
    }
    
    [Injectable]
    public class Repository(ApplicationDbContext context) : IGetQuery<Ingredient, Guid>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Ingredient> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GetOrThrowAsyncNoTracking<Ingredient, Guid>(id, cancellationToken);
        }

        public IQueryable<Ingredient> Query() => _context.Set<Ingredient>().AsNoTracking();
    }
}