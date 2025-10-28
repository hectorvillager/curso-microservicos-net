using FluentValidation;
using PizzaApi.Domain.Entities;

namespace PizzaApi.Domain.Validators
{
    public class PizzaValidator : AbstractValidator<Pizza>
    {
        public PizzaValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Pizza name is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Pizza price must be greater than zero.");

            RuleFor(x => x.Ingredients)
                .NotNull().WithMessage("Ingredients are required.")
                .Must(list => list.Count > 0).WithMessage("At least one ingredient is required.");
        }
    }
}

namespace PizzaApi.Domain.Validators
{
    public class IngredientValidator : AbstractValidator<Ingredient>
    {
        public IngredientValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ingredient name is required.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Ingredient quantity must be greater than zero.");
        }
    }
}