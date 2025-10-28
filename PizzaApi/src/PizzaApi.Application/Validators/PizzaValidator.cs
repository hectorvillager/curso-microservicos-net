using FluentValidation;
using PizzaApi.Application.DTOs;

namespace PizzaApi.Application.Validators
{
    public class PizzaDtoValidator : AbstractValidator<PizzaDto>
    {
        public PizzaDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description is too long.");
        }
    }
}