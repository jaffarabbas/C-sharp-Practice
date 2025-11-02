using ApiTemplate.Security;
using FluentValidation;
using Shared.Dtos;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        // SECURITY: Enforce strong password policy for password changes
        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MustBeStrongPassword(); // Uses comprehensive password policy

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        // Ensure new password is different from current password
        RuleFor(x => x)
            .Must(x => x.NewPassword != x.CurrentPassword)
            .WithMessage("New password must be different from the current password.")
            .When(x => !string.IsNullOrEmpty(x.NewPassword) && !string.IsNullOrEmpty(x.CurrentPassword));
    }
}