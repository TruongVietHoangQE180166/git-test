using FluentValidation;
using TruyenCV.Application.Modules.Auth.DTOs.Requests;
using TruyenCV.Domain.Constants;

namespace TruyenCV.Application.Modules.Auth.Validators;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email is required.")
            .MaximumLength(UserConstants.EmailMaxLength).WithMessage($"Email cannot exceed {UserConstants.EmailMaxLength} characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(UserConstants.UsernameMinLength).WithMessage($"Username must be at least {UserConstants.UsernameMinLength} characters.")
            .MaximumLength(UserConstants.UsernameMaxLength).WithMessage($"Username cannot exceed {UserConstants.UsernameMaxLength} characters.")
            .Matches("^[a-zA-Z0-9_]*$").WithMessage("Username can only contain letters, numbers, and underscores.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
            
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(ProfileConstants.FullNameMaxLength).WithMessage($"Full Name cannot exceed {ProfileConstants.FullNameMaxLength} characters.");
    }
}
