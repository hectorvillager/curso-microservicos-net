namespace PizzaApi.Domain.Aggregates
{
    using System.Collections.Generic;
    using PizzaApi.Domain.Entities;

    public class OrderAggregate
    {
        public int Id { get; private set; }
        public List<Pizza> Pizzas { get; private set; }
        public decimal TotalPrice { get; private set; }

        public OrderAggregate()
        {
            Pizzas = new List<Pizza>();
        }

        public void AddPizza(Pizza pizza)
        {
            Pizzas.Add(pizza);
            CalculateTotalPrice();
        }

        public void RemovePizza(Pizza pizza)
        {
            Pizzas.Remove(pizza);
            CalculateTotalPrice();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = 0;
            foreach (var pizza in Pizzas)
            {
                TotalPrice += pizza.Price;
            }
        }
    }
}