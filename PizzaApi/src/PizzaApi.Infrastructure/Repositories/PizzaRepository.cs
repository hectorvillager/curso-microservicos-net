using PizzaApi.Domain.Entities;
using PizzaApi.Domain.Repositories;
using PizzaApi.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PizzaApi.Infrastructure.Repositories
{
    public class PizzaRepository : IPizzaRepository
    {
        private readonly PizzaDbContext _context;

        public PizzaRepository(PizzaDbContext context)
        {
            _context = context;
        }

        public async Task<Pizza> GetPizzaByIdAsync(int id)
        {
            return await _context.Pizzas.FindAsync(id);
        }

        public async Task<IEnumerable<Pizza>> GetAllPizzasAsync()
        {
            return await _context.Pizzas.ToListAsync();
        }

        public async Task AddPizzaAsync(Pizza pizza)
        {
            await _context.Pizzas.AddAsync(pizza);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePizzaAsync(Pizza pizza)
        {
            _context.Pizzas.Update(pizza);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePizzaAsync(int id)
        {
            var pizza = await GetPizzaByIdAsync(id);
            if (pizza != null)
            {
                _context.Pizzas.Remove(pizza);
                await _context.SaveChangesAsync();
            }
        }
    }
}