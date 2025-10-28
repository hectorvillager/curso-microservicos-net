using PizzaApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaApi.Domain.Repositories
{
    public interface IPizzaRepository
    {
        Task<Pizza> GetPizzaByIdAsync(int id);
        Task<IEnumerable<Pizza>> GetAllPizzasAsync();
        Task AddPizzaAsync(Pizza pizza);
        Task UpdatePizzaAsync(Pizza pizza);
        Task DeletePizzaAsync(int id);
    }
}