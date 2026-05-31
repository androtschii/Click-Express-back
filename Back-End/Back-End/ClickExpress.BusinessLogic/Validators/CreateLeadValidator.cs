using FluentValidation;
using ClickExpress.Domain.Models.Lead;

namespace ClickExpress.BusinessLogic.Validators
{
    public class CreateLeadValidator : AbstractValidator<CreateLeadDTO>
    {
        public CreateLeadValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100).WithMessage("Full name must not exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(150);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[\+\d\s\-\(\)]{7,20}$").WithMessage("Invalid phone number");

            RuleFor(x => x.Origin)
                .NotEmpty().WithMessage("Origin is required")
                .MaximumLength(200);

            RuleFor(x => x.Destination)
                .NotEmpty().WithMessage("Destination is required")
                .MaximumLength(200);

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Weight must be greater than 0")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.PickupDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date).WithMessage("Pickup date cannot be in the past")
                .When(x => x.PickupDate.HasValue);

            RuleFor(x => x.Message)
                .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters")
                .When(x => x.Message != null);
        }
    }
}
