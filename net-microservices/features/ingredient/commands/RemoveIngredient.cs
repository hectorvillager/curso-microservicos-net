using Microsoft.EntityFrameworkCore;
using webapi.common;
using webapi.common.dependencyinjection;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
using webapi.infrastructure;

namespace webapi.features.ingredient.commands;

public class RemoveIngredient : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/ingredientes/{id:guid}", async (IService service, Guid id) =>
        {
            await service.Handler(id);
            return Results.NoContent();
        })        
        .WithOpenApi()
        .WithName("RemoveIngredient")
        .WithSummary("Eliminar un ingrediente")
        .WithDescription("Endpoint para eliminar un ingrediente por su ID")
        .WithTags("Ingredientes")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public interface IService
    {
        Task Handler(Guid id);
    }

    [Injectable]
    public class Service(IRemove<Ingredient, Guid> repository) : IService
    {
        private readonly IRemove<Ingredient, Guid> _repository = repository;

        public async Task Handler(Guid id)
        {
            //var ingredient = await _repository.GetAsync(id);
            
            await _repository.RemoveAsync(new Ingredient(id));
        }
    }
    
    [Injectable]
    public class Repository(ApplicationDbContext context) : IRemove<Ingredient, Guid>
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Ingredient> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.GetOrThrowAsync<Ingredient, Guid>(id, cancellationToken);
        }

        public async Task RemoveAsync(Ingredient entity, CancellationToken cancellationToken = default)
        {
            //_context.Remove(entity);
            _context.Attach(entity);
            _context.Remove(entity);            
            
            //_context.Entry(entity).State = EntityState.Deleted;
            //await _context.Ingredients.Where(i => i.Id.Equals(entity.Id)).ExecuteDeleteAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

       
    }
}