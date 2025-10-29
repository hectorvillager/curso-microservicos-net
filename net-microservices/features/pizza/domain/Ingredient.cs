
/*using webapi.common.domain;

namespace webapi.features.pizza.domain;

public class Ingredient : Entity
{
    protected Ingredient(Guid id, string name, decimal cost) : base(id)
    {
        IngredientValidator.ValidateIngredientData(name, cost);
        
        Name = name;
        Cost = cost;
    }

    public string Name { get; protected set; }
    public decimal Cost { get; protected set; }

    public void Update(string name, decimal cost)
    {
        //Eventos del dominio
        IngredientValidator.ValidateIngredientData(name, cost);
        
        Name = name;
        Cost = cost;
    }

    public static Ingredient Create(Guid id, string name, decimal cost)
    {
        var ingredient = new Ingredient(id, name, cost);
        //Eventos del dominio
        return ingredient;
    }
}

public static class IngredientValidator
{
    public static void ValidateIngredientData(string name, decimal cost)
    {
        ValidateName(name);
        ValidateCost(cost);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("El nombre del ingrediente es requerido.", nameof(name));
        }

        if (name.Length < 2)
        {
            throw new ArgumentException("El nombre debe tener al menos 2 caracteres.", nameof(name));
        }

        if (name.Length > 50)
        {
            throw new ArgumentException("El nombre no puede exceder los 50 caracteres.", nameof(name));
        }
    }

    private static void ValidateCost(decimal cost)
    {
        if (cost < 0)
        {
            throw new ArgumentException("El costo del ingrediente no puede ser negativo.", nameof(cost));
        }

        if (cost == 0)
        {
            throw new ArgumentException("El costo del ingrediente debe ser mayor a cero.", nameof(cost));
        }

        if (cost > 10000)
        {
            throw new ArgumentException("El costo del ingrediente no puede exceder los 10,000.", nameof(cost));
        }
    }
}*/

using FluentValidation;
using webapi.common.domain;

namespace webapi.features.pizza.domain;

public class Ingredient : Entity
{
    private static readonly IngredientValidator _validator = new();

    public string Name { get; protected set; } = string.Empty;
    public decimal Cost { get; protected set; }

    protected Ingredient(Guid id, string name, decimal cost) : base(id)
    {
        ValidateAndSet(name, cost);
    }

    public void Update(string name, decimal cost)
    {
        ValidateAndSet(name, cost);
    }

    private void ValidateAndSet(string name, decimal cost)
    {
        // Validación temprana con tupla antes de asignar valores
        _validator.ValidateAndThrow((name, cost));
        
        Name = name;
        Cost = cost;
    }

    public static Ingredient Create(Guid id, string name, decimal cost)
    {
        var ingredient = new Ingredient(id, name, cost);
        //Eventos del dominio
        return ingredient;
    }
}

public class IngredientValidator : AbstractValidator<(string Name, decimal Cost)>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("El nombre del ingrediente es requerido.")
            .MinimumLength(2)
            .WithMessage("El nombre debe tener al menos 2 caracteres.")
            .MaximumLength(50)
            .WithMessage("El nombre no puede exceder los 50 caracteres.");

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .WithMessage("El costo del ingrediente debe ser mayor a cero.")
            .LessThanOrEqualTo(10000)
            .WithMessage("El costo del ingrediente no puede exceder los 10,000.");
    }
}