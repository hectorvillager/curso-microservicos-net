using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.features.pizza.domain;

namespace webapi.infrastructure.configuration;

public class PizzaConfigurarion : IEntityTypeConfiguration<Pizza>
{
    public void Configure(EntityTypeBuilder<Pizza> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasMany(p => p.Ingredients).WithMany();
    }
}