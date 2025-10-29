using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using webapi.features.pizza.domain;

namespace webapi.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Cost)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.HasIndex(i => i.Name)
            .IsUnique();
    }
}