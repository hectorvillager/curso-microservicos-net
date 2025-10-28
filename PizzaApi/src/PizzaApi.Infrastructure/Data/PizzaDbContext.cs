using Microsoft.EntityFrameworkCore;
using PizzaApi.Domain.Entities;

namespace PizzaApi.Infrastructure.Data
{
    public class PizzaDbContext : DbContext
    {
        public PizzaDbContext(DbContextOptions<PizzaDbContext> options) : base(options)
        {
        }

        public DbSet<Pizza> Pizzas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pizza>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Description)
                .HasMaxLength(500);

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Url)
                .HasMaxLength(200);

            modelBuilder.Entity<Pizza>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Additional configurations can be added here
        }
    }
}