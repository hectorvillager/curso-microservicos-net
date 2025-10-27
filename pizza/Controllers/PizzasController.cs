using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PizzaApi.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class PizzasController : ControllerBase
    {
        // Simulación de base de datos en memoria
        private static readonly List<Ingredient> IngredientsDb = new()
        {
            new Ingredient { Id = 1, Name = "Tomato", Cost = 0.5m },
            new Ingredient { Id = 2, Name = "Cheese", Cost = 1.0m },
            new Ingredient { Id = 3, Name = "Ham", Cost = 1.5m }
        };

        private static readonly List<Pizza> PizzasDb = new();

        // GET /v1/pizzas
        [HttpGet]
        public IActionResult GetAll([FromQuery] string name = null)
        {
            var pizzas = string.IsNullOrEmpty(name)
                ? PizzasDb
                : PizzasDb.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(pizzas);
        }

        // GET /v1/pizzas/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var pizza = PizzasDb.FirstOrDefault(p => p.Id == id);
            if (pizza == null)
                return NotFound(Error("Pizza not found", $"/v1/pizzas/{id}", 404));
            return Ok(pizza);
        }

        // POST /v1/pizzas
        [HttpPost]
        public IActionResult Create([FromBody] PizzaCreateRequest req)
        {
            if (string.IsNullOrEmpty(req.Name) || req.Ingredients == null || !req.Ingredients.Any())
                return UnprocessableEntity(Error("Missing required fields", "/v1/pizzas", 422));

            var pizza = new Pizza
            {
                Id = PizzasDb.Count > 0 ? PizzasDb.Max(p => p.Id) + 1 : 1,
                Name = req.Name,
                Description = req.Description,
                Url = req.Url,
                Ingredients = IngredientsDb.Where(i => req.Ingredients.Contains(i.Id)).ToList()
            };
            pizza.Price = Math.Round(pizza.Ingredients.Sum(i => i.Cost) * 1.2m, 2); // +20% beneficio

            PizzasDb.Add(pizza);
            return CreatedAtAction(nameof(GetById), new { id = pizza.Id }, pizza);
        }

        // PUT /v1/pizzas/{id}
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] PizzaCreateRequest req)
        {
            var pizza = PizzasDb.FirstOrDefault(p => p.Id == id);
            if (pizza == null)
                return NotFound(Error("Pizza not found", $"/v1/pizzas/{id}", 404));

            pizza.Name = req.Name;
            pizza.Description = req.Description;
            pizza.Url = req.Url;
            pizza.Ingredients = IngredientsDb.Where(i => req.Ingredients.Contains(i.Id)).ToList();
            pizza.Price = Math.Round(pizza.Ingredients.Sum(i => i.Cost) * 1.2m, 2);

            return Ok(pizza);
        }

        // DELETE /v1/pizzas/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var pizza = PizzasDb.FirstOrDefault(p => p.Id == id);
            if (pizza == null)
                return NotFound(Error("Pizza not found", $"/v1/pizzas/{id}", 404));
            PizzasDb.Remove(pizza);
            return NoContent();
        }

        private object Error(string message, string path, int status) => new
        {
            path,
            message,
            status,
            timestamp = DateTime.UtcNow
        };
    }

    public class Pizza
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }
        public List<Ingredient> Ingredients { get; set; }
    }

    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
    }

    public class PizzaCreateRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public List<int> Ingredients { get; set; }
    }
}