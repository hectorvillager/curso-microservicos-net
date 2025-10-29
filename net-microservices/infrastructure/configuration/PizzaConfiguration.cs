using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.features.pizza.domain;

namespace webapi.Data.Configurations;

public class PizzaConfiguration : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> builder)
    {
        builder.ToTable("Pizzas");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Url)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(p => p.Name);

        // Configurar relación Many-to-Many sin propiedades de navegación
        builder.HasMany<Ingredient>()
            .WithMany();
    }
}