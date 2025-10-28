namespace PizzaApi.Domain.Entities
{
    public class Pizza
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Url { get; private set; }
        public decimal Price { get; private set; }
        public List<Ingredient> Ingredients { get; private set; }

        public Pizza(string name, string description, string url, decimal price, List<Ingredient> ingredients)
        {
            Name = name;
            Description = description;
            Url = url;
            Price = price;
            Ingredients = ingredients ?? new List<Ingredient>();
        }

        // Additional methods for business logic can be added here
    }

    public class Ingredient
    {
        public string Name { get; private set; }
        public decimal Quantity { get; private set; }

        public Ingredient(string name, decimal quantity)
        {
            Name = name;
            Quantity = quantity;
        }
    }
}