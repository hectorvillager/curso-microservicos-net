using Microsoft.AspNetCore.Mvc;
using webapi.common;
using webapi.common.dependencyinjection;
using System.ComponentModel.DataAnnotations;
using webapi.features.pizza.domain;
using webapi.infrastructure;
using Microsoft.EntityFrameworkCore;

namespace webapi.features.ingredient.commands;

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
        .WithName("GetIngredients")
        .WithSummary("Recupera ingredientes")
        .WithDescription("Endpoint para recueperar ingredientes")
        .WithTags("Ingredientes")
        .Produces<List<Response>>(StatusCodes.Status200OK);
    }

    public interface IService
    {
        Task<IEnumerable<Response>> Handler();
    }

    [Injectable]
    public class Service(IQuery repository) : IService
    {
        private readonly IQuery _repository = repository;

        public async Task<IEnumerable<Response>> Handler()
        {

            var ingredients = await _repository.GetAll();

            return ingredients.Select(ingredient => new Response
            {

                Id = ingredient.Id,
                Name = ingredient.Name,
                Cost = ingredient.Cost
            });

        }
    }

    public interface IQuery
    {
        Task<IQueryable<Ingredient>> GetAll();
    }


    [Injectable]
    public class Repository(ApplicationDbContext context) : IQuery
    {
        private readonly ApplicationDbContext _context = context;


        public Task<IQueryable<Ingredient>> GetAll()
        {
            return Task.FromResult(_context.Ingredients.AsNoTracking());
        }
    }
}