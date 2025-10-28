using PizzaApi.Domain.Entities;
using PizzaApi.Domain.Repositories;
using PizzaApi.Application.DTOs;
using PizzaApi.Application.Validators;
using PizzaApi.Domain.Validators;
using FluentValidation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaApi.Application.Services
{
    public class PizzaService
    {
        private readonly IPizzaRepository _pizzaRepository;
        private readonly PizzaDtoValidator _pizzaDtoValidator = new PizzaDtoValidator();
        private readonly PizzaValidator _pizzaValidator = new PizzaValidator();
        private readonly IngredientValidator _ingredientValidator = new IngredientValidator();

        public PizzaService(IPizzaRepository pizzaRepository)
        {
            _pizzaRepository = pizzaRepository;
        }

        public async Task<IEnumerable<PizzaDto>> GetAllPizzasAsync()
        {
            var pizzas = await _pizzaRepository.GetAllPizzasAsync();
            return MapToPizzaDtos(pizzas);
        }

        public async Task<PizzaDto> GetPizzaByIdAsync(int id)
        {
            var pizza = await _pizzaRepository.GetPizzaByIdAsync(id);
            return MapToPizzaDto(pizza);
        }

        public async Task<PizzaDto> CreatePizzaAsync(PizzaDto pizzaDto)
        {
            // Validar DTO
            _pizzaDtoValidator.ValidateAndThrow(pizzaDto);

            var pizza = MapToPizza(pizzaDto);

            // Validar entidad Pizza
            _pizzaValidator.ValidateAndThrow(pizza);

            // Validar cada ingrediente
            foreach (var ingredient in pizza.Ingredients)
            {
                _ingredientValidator.ValidateAndThrow(ingredient);
            }

            await _pizzaRepository.AddPizzaAsync(pizza);
            return MapToPizzaDto(pizza);
        }

        public async Task<PizzaDto> UpdatePizzaAsync(int id, PizzaDto pizzaDto)
        {
            _pizzaDtoValidator.ValidateAndThrow(pizzaDto);

            var pizza = await _pizzaRepository.GetPizzaByIdAsync(id);
            if (pizza == null) return null;

            UpdatePizzaFromDto(pizza, pizzaDto);

            _pizzaValidator.ValidateAndThrow(pizza);

            foreach (var ingredient in pizza.Ingredients)
            {
                _ingredientValidator.ValidateAndThrow(ingredient);
            }

            await _pizzaRepository.UpdatePizzaAsync(pizza);
            return MapToPizzaDto(pizza);
        }

        public async Task<bool> DeletePizzaAsync(int id)
        {
            var pizza = await _pizzaRepository.GetPizzaByIdAsync(id);
            if (pizza == null) return false;

            await _pizzaRepository.DeletePizzaAsync(pizza.Id);
            return true;
        }

        public async Task<bool> AddIngredientAsync(int pizzaId, string ingredientName, decimal quantity)
        {
            var pizza = await _pizzaRepository.GetPizzaByIdAsync(pizzaId);
            if (pizza == null) return false;

            var ingredient = Ingredient.Create(ingredientName, quantity);
            _ingredientValidator.ValidateAndThrow(ingredient);

            pizza.Ingredients.Add(ingredient);
            _pizzaValidator.ValidateAndThrow(pizza);

            await _pizzaRepository.UpdatePizzaAsync(pizza);
            return true;
        }

        public async Task<bool> RemoveIngredientAsync(int pizzaId, string ingredientName)
        {
            var pizza = await _pizzaRepository.GetPizzaByIdAsync(pizzaId);
            if (pizza == null) return false;

            var ingredient = pizza.Ingredients.Find(i => i.Name == ingredientName);
            if (ingredient == null) return false;

            pizza.Ingredients.Remove(ingredient);
            _pizzaValidator.ValidateAndThrow(pizza);

            await _pizzaRepository.UpdatePizzaAsync(pizza);
            return true;
        }

        public async Task<List<Ingredient>> GetIngredientsAsync(int pizzaId)
        {
            var pizza = await _pizzaRepository.GetPizzaByIdAsync(pizzaId);
            return pizza?.Ingredients ?? new List<Ingredient>();
        }

        private IEnumerable<PizzaDto> MapToPizzaDtos(IEnumerable<Pizza> pizzas)
        {
            var dtos = new List<PizzaDto>();
            foreach (var pizza in pizzas)
            {
                dtos.Add(MapToPizzaDto(pizza));
            }
            return dtos;
        }

        private PizzaDto MapToPizzaDto(Pizza pizza)
        {
            if (pizza == null) return null;
            return new PizzaDto
            {
                Id = pizza.Id,
                Name = pizza.Name,
                Description = pizza.Description,
                Url = pizza.Url,
                Price = pizza.Price,
                Size = null // Si tienes un atributo Size en Pizza, asígnalo aquí
                // Puedes agregar Ingredients si tu DTO lo necesita
            };
        }

        private Pizza MapToPizza(PizzaDto pizzaDto)
        {
            if (pizzaDto == null) return null;
            // Si tienes Ingredients en el DTO, mapea aquí
            return new Pizza(
                pizzaDto.Name,
                pizzaDto.Description,
                pizzaDto.Url,
                pizzaDto.Price,
                new List<Ingredient>() // Puedes mapear ingredientes si el DTO los tiene
            );
        }

        private void UpdatePizzaFromDto(Pizza pizza, PizzaDto pizzaDto)
        {
            if (pizza == null || pizzaDto == null) return;
            // Si tus propiedades tienen setters privados, puedes usar métodos internos o reflejar la lógica aquí
            // Ejemplo usando métodos internos (deberías tenerlos en la entidad Pizza)
            // pizza.UpdateName(pizzaDto.Name);
            // pizza.UpdateDescription(pizzaDto.Description);
            // pizza.UpdateUrl(pizzaDto.Url);
            // pizza.UpdatePrice(pizzaDto.Price);

            // Si tienes setters públicos, puedes hacer:
            // pizza.Name = pizzaDto.Name;
            // pizza.Description = pizzaDto.Description;
            // pizza.Url = pizzaDto.Url;
            // pizza.Price = pizzaDto.Price;
            // Actualiza ingredientes si lo necesitas
        }        
    }
}