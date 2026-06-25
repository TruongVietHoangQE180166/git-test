using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using TruyenCV.Application.Modules.Profile.DTOs.Requests;
using TruyenCV.Application.Modules.Profile.DTOs.Responses;
using TruyenCV.Application.Modules.Profile.Interfaces.Services;
using TruyenCV.Application.Modules.Profile.Interfaces.Repositories;
using TruyenCV.Application.Modules.User.Interfaces.Repositories;
using TruyenCV.Shared.Exceptions;
using TruyenCV.Shared.Requests;
using TruyenCV.Shared.Responses;

namespace TruyenCV.Application.Modules.Profile.Services;

public class ProfileService : IProfileService
{
    private readonly IProfileRepository _profileRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public ProfileService(
        IProfileRepository profileRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _profileRepository = profileRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ProfileResponse> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, includeUser: true, ct);
        if (profile == null)
        {
            throw new NotFoundException($"Profile for User ID {userId} was not found.");
        }
        return _mapper.Map<ProfileResponse>(profile);
    }

    public async Task<ProfileResponse> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        var profile = await _profileRepository.GetBySlugAsync(slug, ct);
        if (profile == null)
        {
            throw new NotFoundException($"Profile with slug '{slug}' was not found.");
        }
        return _mapper.Map<ProfileResponse>(profile);
    }

    public async Task<PaginatedResponse<ProfileResponse>> GetPagedAsync(PaginationRequest request, CancellationToken ct = default)
    {
        var (profiles, totalCount) = await _profileRepository.GetPagedAsync(
            request.PageNumber ?? 1,
            request.PageSize ?? 10,
            request.SortField,
            request.SortDirection,
            request.SearchTerm,
            ct);

        foreach (var profile in profiles)
        {
            if (profile.User == null)
            {
                profile.User = await _userRepository.GetByIdAsync(profile.UserId, ct) ?? null!;
            }
        }

        var mappedProfiles = _mapper.Map<IReadOnlyList<ProfileResponse>>(profiles);
        return PaginatedResponse<ProfileResponse>.Create(mappedProfiles, totalCount, request.PageNumber ?? 1, request.PageSize ?? 10);
    }

    public async Task<ProfileResponse> UpdateAsync(Guid userId, UpdateProfileRequest request, CancellationToken ct = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, includeUser: true, ct);
        if (profile == null)
        {
            throw new NotFoundException($"Profile for User ID {userId} was not found.");
        }

        profile.FullName = request.FullName;
        profile.Bio = request.Bio;
        profile.AvatarUrl = request.AvatarUrl;
        profile.PhoneNumber = request.PhoneNumber;
        profile.DateOfBirth = request.DateOfBirth;
        profile.Gender = request.Gender;
        profile.IsPublic = request.IsPublic;

        // Slug generation
        var baseSlug = Slugify(request.FullName);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = profile.User?.Username.ToLowerInvariant() ?? Guid.NewGuid().ToString("N");
        }

        var uniqueSlug = baseSlug;
        int suffix = 1;
        while (await _profileRepository.ExistsBySlugAsync(uniqueSlug, ct))
        {
            var existingWithSlug = await _profileRepository.GetBySlugAsync(uniqueSlug, ct);
            if (existingWithSlug != null && existingWithSlug.Id == profile.Id)
            {
                break;
            }
            uniqueSlug = $"{baseSlug}-{suffix}";
            suffix++;
        }
        profile.Slug = uniqueSlug;

        profile.UpdatedAt = DateTime.UtcNow;
        _profileRepository.Update(profile);
        await _profileRepository.SaveChangesAsync(ct);

        return _mapper.Map<ProfileResponse>(profile);
    }

    private string Slugify(string phrase)
    {
        if (string.IsNullOrEmpty(phrase)) return string.Empty;

        // Remove diacritics for Vietnamese
        string str = RemoveDiacritics(phrase).ToLowerInvariant();

        // Remove invalid chars, replace spaces/underscores with hyphens
        var sb = new StringBuilder();
        foreach (char c in str)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
            else if (c == ' ' || c == '-' || c == '_')
            {
                sb.Append('-');
            }
        }

        var result = sb.ToString();
        while (result.Contains("--"))
        {
            result = result.Replace("--", "-");
        }

        return result.Trim('-');
    }

    private string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                // Specifically map Vietnamese characters that FormD doesn't fully strip in standard ways if needed,
                // but standard FormD handles most accents. For 'đ' / 'Đ', we need manual mapping:
                if (c == 'đ') stringBuilder.Append('d');
                else if (c == 'Đ') stringBuilder.Append('d');
                else stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}
