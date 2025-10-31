using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.pizza.commands;

public class CreatePizza : IFeatureModule
{
    public record struct Request(
        [Required][property: Required] string Name,
        [Required][property: Required] string Description,
        [Required][property: Required] string Url,
        [Required][property: Required] IEnumerable<Guid> Ingredients
    );

    public record struct IngredientResponse(
        [Required][property: Required] Guid Id,
        [Required][property: Required] string Name
    );

    public record struct Response(
        [Required][property: Required] Guid Id,
        [Required][property: Required] string Name,
        [Required][property: Required] string Description,
        [Required][property: Required] string Url,
        [Required][property: Required] decimal Price,
        [Required][property: Required] IEnumerable<IngredientResponse> Ingredients
    );

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/pizzas", async (IService service,  Request request) =>
        {
            var response = await service.Handler(request);
            return Results.Ok(response);
        })
        .WithOpenApi()
        .WithName("CreatePizza")
        .WithSummary("Crear una nueva pizza")
        .WithDescription("Endpoint para crear una nueva pizza con sus ingredientes")
        .WithTags("Pizzas")
        .Produces<Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public interface IService
    {
        Task<Response> Handler(Request request);
    }

    [Injectable]
    public class Service(
        IAdd<Pizza> pizzaRepository,
        IGetOrThrowAsync ingredientRepository) : IService
    {
        private readonly IAdd<Pizza> _pizzaRepository = pizzaRepository;
        private readonly IGetOrThrowAsync _ingredientRepository = ingredientRepository;

        public async Task<Response> Handler(Request request)
        {
            var pizza = Pizza.Create(Guid.NewGuid(), request.Name, request.Description, request.Url);

            foreach (var ingredientId in request.Ingredients)
            {
                var ingredient = await _ingredientRepository.GetOrThrowAsync<Ingredient, Guid>(ingredientId);
                pizza.AddIngredient(ingredient);
            }

            await _pizzaRepository.AddAsync(pizza);
           

            var response = new Response(
                pizza.Id,
                pizza.Name,
                pizza.Description,
                pizza.Url,
                pizza.Price,
                pizza.Ingredients.Select(i => new IngredientResponse(i.Id, i.Name))
            );

            return response;
        }
    }

    [Injectable]
    public class Repository(ApplicationDbContext context) : IAdd<Pizza>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task AddAsync(Pizza entity, CancellationToken cancellationToken = default)
        {            
            _context.Attach(entity);
            _context.Entry(entity).State = EntityState.Added;        
             //Marca todas las entidades relacionadas en estado Added 
            //await _context.AddAsync(entity)   
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}