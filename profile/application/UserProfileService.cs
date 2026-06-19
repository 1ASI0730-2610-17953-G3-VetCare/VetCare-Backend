using VetCare.profile.domain.model.aggregates;
using VetCare.profile.domain.repositories;

namespace VetCare.profile.application;

public class UserProfileService
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpeg"
    };

    private readonly IUserProfileRepository _profileRepository;

    public UserProfileService(IUserProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task<UserProfile> GetOrCreateProfileAsync(int userId)
    {
        var profile = await _profileRepository.FindByUserIdAsync(userId);
        if (profile == null)
        {
            profile = new UserProfile(userId, "light", "es", true);
            await _profileRepository.AddAsync(profile);
        }
        return profile;
    }

    public async Task<UserProfile> UpdateProfileAsync(int userId, string theme, string language, bool receiveNotifications)
    {
        var profile = await GetOrCreateProfileAsync(userId);
        profile.UpdatePreferences(theme, language, receiveNotifications);
        await _profileRepository.UpdateAsync(profile);
        return profile;
    }

    public async Task<string> UploadAvatarAsync(int userId, Stream fileStream, string contentType, string webRootPath)
    {
        if (!AllowedContentTypes.Contains(contentType))
            throw new AvatarUploadException("Only PNG and JPEG images are allowed.");

        var uploadsDir = Path.Combine(webRootPath, "uploads", "avatars");
        Directory.CreateDirectory(uploadsDir);

        foreach (var existing in Directory.GetFiles(uploadsDir, $"{userId}.*"))
            File.Delete(existing);

        var extension = contentType.Equals("image/png", StringComparison.OrdinalIgnoreCase) ? ".png" : ".jpg";
        var fileName = $"{userId}{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);

        await using (var output = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(output);
        }

        var avatarUrl = $"/uploads/avatars/{fileName}";
        var profile = await GetOrCreateProfileAsync(userId);
        profile.UpdateAvatarUrl(avatarUrl);
        await _profileRepository.UpdateAsync(profile);
        return avatarUrl;
    }
}
