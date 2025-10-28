namespace PizzaApi.Application.DTOs
{
    public class PizzaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; } // Assuming Size is represented as a string for simplicity
    }
}