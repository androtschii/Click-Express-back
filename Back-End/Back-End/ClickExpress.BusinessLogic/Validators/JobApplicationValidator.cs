using FluentValidation;
using ClickExpress.Domain.Models.JobApplication;

namespace ClickExpress.BusinessLogic.Validators
{
    public class CreateJobApplicationValidator : AbstractValidator<CreateJobApplicationDTO>
    {
        public CreateJobApplicationValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(150);

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .Matches(@"^[\+\d\s\-\(\)]{7,20}$").WithMessage("Invalid phone number");

            RuleFor(x => x.Position)
                .NotEmpty().WithMessage("Position is required")
                .MaximumLength(100);

            RuleFor(x => x.Message)
                .MaximumLength(2000)
                .When(x => x.Message != null);
        }
    }
}
