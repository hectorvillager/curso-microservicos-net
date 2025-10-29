using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.infrastructure;
using webapi.common.dependencyinjection;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.ingredient.commands;

public class RemoveIngredient : IFeatureModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/ingredientes/{id:guid}", async (IService service, Guid id) =>
        {
            var success = await service.Handler(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithOpenApi()
        .WithName("RemoveIngredient")
        .WithSummary("Eliminar un ingrediente por Id")
        .WithDescription("Endpoint para eliminar un ingrediente específico por su identificador")
        .WithTags("Ingredientes")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }

    public interface IService
    {
        Task<bool> Handler(Guid id);
    }

    [Injectable]
    public class Service : IService
    {
        private readonly IRemove<Ingredient> _repository;
        public Service(IRemove<Ingredient> repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handler(Guid id)
        {
            return await _repository.RemoveAsync(id);
        }
    }

    [Injectable]
    public class Repository : IRemove<Ingredient>
    {
        private readonly ApplicationDbContext _context;
        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient is null) return false;
            _context.Ingredients.Remove(ingredient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

// filepath: src/webapi/common/infrastructure/IRemove.cs
public interface IRemove<T>
{
    Task<bool> RemoveAsync(Guid id);
}