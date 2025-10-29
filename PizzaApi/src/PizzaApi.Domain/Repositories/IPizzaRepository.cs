using PizzaApi.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PizzaApi.Domain.Repositories
{
    public interface IReadRepository<T>
    {
        Task<T> GetPizzaByIdAsync(int id);
        Task<IEnumerable<T>> GetAllPizzasAsync();
    }

    public interface IWriteRepository<T>
    {
        Task AddPizzaAsync(T entity);
        Task UpdatePizzaAsync(T entity);
        Task DeletePizzaAsync(int id);
    }

    public interface IPizzaRepository : IWriteRepository<Pizza>, IReadRepository<Pizza>
    {
        Task<Pizza> GetPizzaByIdAsync(int id);
        Task<IEnumerable<Pizza>> GetAllPizzasAsync();
        Task AddPizzaAsync(Pizza pizza);
        Task UpdatePizzaAsync(Pizza pizza);
        Task DeletePizzaAsync(int id);
    }
}