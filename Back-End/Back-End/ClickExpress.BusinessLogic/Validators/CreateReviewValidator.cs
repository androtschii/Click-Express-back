using FluentValidation;
using ClickExpress.Domain.Models.Review;

namespace ClickExpress.BusinessLogic.Validators
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDTO>
    {
        public CreateReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Review text is required")
                .MinimumLength(10).WithMessage("Review must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Review must not exceed 2000 characters");

            RuleFor(x => x.Role)
                .MaximumLength(100)
                .When(x => x.Role != null);

            RuleFor(x => x.Location)
                .MaximumLength(100)
                .When(x => x.Location != null);
        }
    }

    public class UpdateReviewValidator : AbstractValidator<UpdateReviewDTO>
    {
        public UpdateReviewValidator()
        {
            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Text)
                .NotEmpty().WithMessage("Review text is required")
                .MinimumLength(10).WithMessage("Review must be at least 10 characters")
                .MaximumLength(2000).WithMessage("Review must not exceed 2000 characters");
        }
    }
}
