using Microsoft.EntityFrameworkCore;
using webapi.common.dependencyinjection;
using webapi.common.domain;
using webapi.common.infrastructure;
using webapi.features.pizza.domain;
namespace webapi.infrastructure;

[Injectable]
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IGetOrThrowAsync
{
    public DbSet<Pizza> Pizzas { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }

    public async Task<T> GetOrThrowAsync<T, ID>(ID id, CancellationToken cancellationToken = default, bool tracking = true) where T : Entity
    {
        var query = Set<T>().AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }
        var entity = await query.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        return entity ?? throw new KeyNotFoundException($"{typeof(T).Name} with ID '{id}' not found.");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }
}