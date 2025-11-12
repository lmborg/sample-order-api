using FluentValidation;

namespace Application.Products.Commands;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty();
        RuleFor(c => c.Price).GreaterThan(0);
        RuleFor(c => c.StockQuantity).GreaterThanOrEqualTo(0);
    }
}