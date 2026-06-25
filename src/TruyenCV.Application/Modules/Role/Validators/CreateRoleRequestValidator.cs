using FluentValidation;
using TruyenCV.Application.Modules.Role.DTOs.Requests;

namespace TruyenCV.Application.Modules.Role.Validators;

public class CreateRoleRequestValidator : AbstractValidator<CreateRoleRequest>
{
    public CreateRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MinimumLength(3).WithMessage("Role name must be at least 3 characters.")
            .MaximumLength(50).WithMessage("Role name cannot exceed 50 characters.");
    }
}
