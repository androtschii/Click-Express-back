using FluentValidation;
using ClickExpress.Domain.Models.Order;
using ClickExpress.Domain.Models.Cart;
using ClickExpress.Domain.Models.SavedLoad;

// ReSharper disable once CheckNamespace

namespace ClickExpress.BusinessLogic.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDTO>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("A valid product must be selected");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Notes must not exceed 1000 characters")
                .When(x => x.Notes != null);

            RuleFor(x => x.PickupAddress)
                .MaximumLength(300).WithMessage("Pickup address must not exceed 300 characters")
                .When(x => x.PickupAddress != null);

            RuleFor(x => x.DeliveryAddress)
                .MaximumLength(300).WithMessage("Delivery address must not exceed 300 characters")
                .When(x => x.DeliveryAddress != null);

            RuleFor(x => x.PickupDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Pickup date cannot be in the past")
                .When(x => x.PickupDate.HasValue);

            RuleFor(x => x.DeliveryDate)
                .GreaterThan(x => x.PickupDate!.Value)
                .WithMessage("Delivery date must be after pickup date")
                .When(x => x.DeliveryDate.HasValue && x.PickupDate.HasValue);

            RuleFor(x => x.TotalPrice)
                .GreaterThan(0).WithMessage("Total price must be greater than zero")
                .When(x => x.TotalPrice.HasValue);
        }
    }

    public class AddCartItemValidator : AbstractValidator<AddCartItemDTO>
    {
        public AddCartItemValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("A valid product must be selected");

            RuleFor(x => x.Quantity)
                .InclusiveBetween(1, 50).WithMessage("Quantity must be between 1 and 50");
        }
    }

    public class AddSavedLoadValidator : AbstractValidator<AddSavedLoadDTO>
    {
        public AddSavedLoadValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("A valid product must be selected");
        }
    }

    public class UpdateOrderStatusValidator : AbstractValidator<UpdateOrderStatusDTO>
    {
        private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            "Pending", "Confirmed", "InTransit", "Delivered", "Cancelled"
        };

        public UpdateOrderStatusValidator()
        {
            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .Must(s => AllowedStatuses.Contains(s))
                .WithMessage($"Status must be one of: {string.Join(", ", AllowedStatuses)}");
        }
    }
}
