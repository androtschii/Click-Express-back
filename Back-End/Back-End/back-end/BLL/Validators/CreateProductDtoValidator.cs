using FluentValidation;
using back_end.BLL.DTOs;

namespace back_end.BLL.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название обязательно")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Категория обязательна");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Остаток не может быть отрицательным");
        }
    }
}
