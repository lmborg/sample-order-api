using FluentValidation;

namespace Application.Products.Commands;

public class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
{
    public UpdateProductStockCommandValidator()
    {
        RuleFor(c => c.StockQuantity).GreaterThanOrEqualTo(0);
    }
}