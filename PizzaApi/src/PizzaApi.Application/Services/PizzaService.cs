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

        // Mapping methods (not implemented here)
        private IEnumerable<PizzaDto> MapToPizzaDtos(IEnumerable<Pizza> pizzas) => throw new System.NotImplementedException();
        private PizzaDto MapToPizzaDto(Pizza pizza) => throw new System.NotImplementedException();
        private Pizza MapToPizza(PizzaDto pizzaDto) => throw new System.NotImplementedException();
        private void UpdatePizzaFromDto(Pizza pizza, PizzaDto pizzaDto) => throw new System.NotImplementedException();
    }
}