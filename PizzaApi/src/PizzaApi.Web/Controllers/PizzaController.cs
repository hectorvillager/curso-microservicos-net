using Microsoft.AspNetCore.Mvc;
using PizzaApi.Application.DTOs;
using PizzaApi.Application.Services;
using PizzaApi.Application.Validators;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PizzaController : ControllerBase
    {
        private readonly PizzaService _pizzaService;
        private readonly PizzaDtoValidator _pizzaDtoValidator = new PizzaDtoValidator();

        public PizzaController(PizzaService pizzaService)
        {
            _pizzaService = pizzaService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaDto>>> GetAllPizzas()
        {
            var pizzas = await _pizzaService.GetAllPizzasAsync();
            return Ok(pizzas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PizzaDto>> GetPizzaById(int id)
        {
            var pizza = await _pizzaService.GetPizzaByIdAsync(id);
            if (pizza == null)
            {
                return NotFound();
            }
            return Ok(pizza);
        }

        [HttpPost]
        public async Task<ActionResult<PizzaDto>> CreatePizza(PizzaDto pizzaDto)
        {
            var validationResult = _pizzaDtoValidator.Validate(pizzaDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var createdPizza = await _pizzaService.CreatePizzaAsync(pizzaDto);
            return CreatedAtAction(nameof(GetPizzaById), new { id = createdPizza.Id }, createdPizza);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePizza(int id, PizzaDto pizzaDto)
        {
            if (id != pizzaDto.Id)
            {
                return BadRequest();
            }

            var validationResult = _pizzaDtoValidator.Validate(pizzaDto);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            await _pizzaService.UpdatePizzaAsync(id, pizzaDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePizza(int id)
        {
            await _pizzaService.DeletePizzaAsync(id);
            return NoContent();
        }
    }
}