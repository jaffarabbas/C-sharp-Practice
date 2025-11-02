using ApiTemplate.Security;
using FluentValidation;
using Shared.Dtos;

public class TblUsersDtoValidator : AbstractValidator<TblUsersDto>
{
    public TblUsersDtoValidator()
    {
        RuleFor(x => x.Firstname)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.Lastname)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MaximumLength(100).WithMessage("Username must not exceed 100 characters.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters.")
            .Matches(@"^[a-zA-Z0-9_.-]+$").WithMessage("Username can only contain letters, numbers, underscores, dots, and hyphens.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(255).WithMessage("Email must not exceed 255 characters.");

        // SECURITY: Enforce strong password policy for user registration
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MustBeStrongPassword(); // Uses comprehensive password policy
    }
}