using ApiTemplate.Dtos;
using ApiTemplate.Security;
using FluentValidation;
using Shared.Dtos;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100).WithMessage("Username must not exceed 100 characters.");

        // For login, we only check if password is not empty
        // Full strength validation is only for registration/password changes
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}