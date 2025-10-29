using Microsoft.EntityFrameworkCore;
using webapi.Data.Configurations;
using webapi.features.pizza.domain;

namespace webapi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Opción 1: Registrar configuraciones manualmente
        modelBuilder.ApplyConfiguration(new IngredientConfiguration());
        modelBuilder.ApplyConfiguration(new PizzaConfiguration());

        // Opción 2: Registrar todas las configuraciones automáticamente con reflexión
        // Descomenta las siguientes líneas para usar este enfoque:
        
        // var assembly = typeof(ApplicationDbContext).Assembly;
        // modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        // Opción 3: Registrar configuraciones específicas de un namespace con LINQ y reflexión
        // Descomenta las siguientes líneas para usar este enfoque:
        
        // var configurationsNamespace = "TuProyecto.Data.Configurations";
        // var configurationTypes = typeof(ApplicationDbContext).Assembly
        //     .GetTypes()
        //     .Where(t => t.Namespace == configurationsNamespace 
        //         && !t.IsAbstract 
        //         && !t.IsInterface
        //         && t.GetInterfaces().Any(i => 
        //             i.IsGenericType && 
        //             i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
        //     .ToList();
        //
        // foreach (var configurationType in configurationTypes)
        // {
        //     var configurationInstance = Activator.CreateInstance(configurationType);
        //     var applyMethod = typeof(ModelBuilder)
        //         .GetMethods()
        //         .First(m => m.Name == nameof(ModelBuilder.ApplyConfiguration) 
        //             && m.GetParameters().Length == 1
        //             && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));
        //     
        //     var entityType = configurationType.GetInterfaces()
        //         .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
        //         .GetGenericArguments()[0];
        //     
        //     var genericApplyMethod = applyMethod.MakeGenericMethod(entityType);
        //     genericApplyMethod.Invoke(modelBuilder, new[] { configurationInstance });
        // }
    }
}