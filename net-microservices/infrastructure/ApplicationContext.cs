using Microsoft.EntityFrameworkCore;
using webapi.common.dependencyinjection;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
namespace webapi.infrastructure;

[Injectable]
public class ApplicationDbContext : DbContext, IGetOrThrowAsync, IGetOrThrowAsyncNoTracking
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    public async Task<T> GetOrThrowAsync<T, ID>(ID id, CancellationToken cancellationToken = default) where T : class
    {
        var entity = await Set<T>().FindAsync([id!], cancellationToken);
        return entity ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID '{id}' not found.");
    }

    public async Task<T> GetOrThrowAsyncNoTracking<T, ID>(ID id, CancellationToken cancellationToken = default) where T : class
    {
        var entity = await Set<T>()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => EF.Property<ID>(e, "Id").Equals(id), cancellationToken);

        return entity ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID '{id}' not found.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }
}