using FluentValidation;
using back_end.BLL.DTOs;

namespace back_end.BLL.Validators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId должен быть больше 0");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Заметки не длиннее 500 символов");
        }
    }
}
