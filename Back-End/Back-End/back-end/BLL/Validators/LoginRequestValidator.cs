using FluentValidation;
using back_end.Controllers;

namespace back_end.BLL.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Логин обязателен")
                .MinimumLength(3).WithMessage("Логин не короче 3 символов")
                .MaximumLength(50);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен")
                .MinimumLength(6).WithMessage("Пароль не короче 6 символов");
        }
    }
}
