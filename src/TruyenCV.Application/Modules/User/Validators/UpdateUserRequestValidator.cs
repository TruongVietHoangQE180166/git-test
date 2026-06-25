using FluentValidation;
using TruyenCV.Application.Modules.User.DTOs.Requests;
using TruyenCV.Domain.Constants;

namespace TruyenCV.Application.Modules.User.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
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

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role ID is required.");
    }
}
