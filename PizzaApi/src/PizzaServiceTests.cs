using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using PizzaApi.Application.Services;
using PizzaApi.Domain.Entities;
using PizzaApi.Domain.Repositories;

namespace PizzaApi.Tests
{
    public class PizzaServiceTests
    {
        private readonly Mock<IPizzaRepository> _pizzaRepositoryMock;
        private readonly PizzaService _pizzaService;

        public PizzaServiceTests()
        {
            _pizzaRepositoryMock = new Mock<IPizzaRepository>();
            _pizzaService = new PizzaService(_pizzaRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllPizzas_ReturnsAllPizzas()
        {
            // Arrange
            var pizzas = new List<Pizza>
            {
                new Pizza { Id = 1, Name = "Margherita", Price = 8.99M },
                new Pizza { Id = 2, Name = "Pepperoni", Price = 9.99M }
            };
            _pizzaRepositoryMock.Setup(repo => repo.GetAllPizzasAsync()).ReturnsAsync(pizzas);

            // Act
            var result = await _pizzaService.GetAllPizzasAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetPizzaById_ReturnsPizza_WhenPizzaExists()
        {
            // Arrange
            var pizza = new Pizza { Id = 1, Name = "Margherita", Price = 8.99M };
            _pizzaRepositoryMock.Setup(repo => repo.GetPizzaByIdAsync(1)).ReturnsAsync(pizza);

            // Act
            var result = await _pizzaService.GetPizzaByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Margherita", result.Name);
        }

        [Fact]
        public async Task CreatePizza_AddsPizza()
        {
            // Arrange
            var pizza = new Pizza { Name = "Margherita", Price = 8.99M };

            // Act
            await _pizzaService.CreatePizzaAsync(pizza);

            // Assert
            _pizzaRepositoryMock.Verify(repo => repo.AddPizzaAsync(pizza), Times.Once);
        }

        [Fact]
        public async Task UpdatePizza_UpdatesPizza_WhenPizzaExists()
        {
            // Arrange
            var pizza = new Pizza { Id = 1, Name = "Margherita", Price = 8.99M };
            _pizzaRepositoryMock.Setup(repo => repo.GetPizzaByIdAsync(1)).ReturnsAsync(pizza);

            // Act
            pizza.Name = "Updated Margherita";
            await _pizzaService.UpdatePizzaAsync(pizza);

            // Assert
            _pizzaRepositoryMock.Verify(repo => repo.UpdatePizzaAsync(pizza), Times.Once);
        }

        [Fact]
        public async Task DeletePizza_RemovesPizza_WhenPizzaExists()
        {
            // Arrange
            var pizza = new Pizza { Id = 1, Name = "Margherita", Price = 8.99M };
            _pizzaRepositoryMock.Setup(repo => repo.GetPizzaByIdAsync(1)).ReturnsAsync(pizza);

            // Act
            await _pizzaService.DeletePizzaAsync(1);

            // Assert
            _pizzaRepositoryMock.Verify(repo => repo.DeletePizzaAsync(pizza), Times.Once);
        }
    }
}