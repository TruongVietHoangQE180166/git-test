using FluentValidation;
using TruyenCV.Application.Modules.Profile.DTOs.Requests;
using TruyenCV.Domain.Constants;

namespace TruyenCV.Application.Modules.Profile.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full Name is required.")
            .MaximumLength(ProfileConstants.FullNameMaxLength).WithMessage($"Full Name cannot exceed {ProfileConstants.FullNameMaxLength} characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(ProfileConstants.BioMaxLength).WithMessage($"Bio cannot exceed {ProfileConstants.BioMaxLength} characters.");

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(ProfileConstants.AvatarUrlMaxLength).WithMessage($"Avatar URL cannot exceed {ProfileConstants.AvatarUrlMaxLength} characters.");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(ProfileConstants.PhoneNumberMaxLength).WithMessage($"Phone number cannot exceed {ProfileConstants.PhoneNumberMaxLength} characters.");
            
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender option.");
    }
}
