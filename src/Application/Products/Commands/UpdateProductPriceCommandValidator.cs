using FluentValidation;

namespace Application.Products.Commands;

public class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
{
    public UpdateProductPriceCommandValidator()
    {
        RuleFor(c => c.Price).GreaterThan(0);
    }
}