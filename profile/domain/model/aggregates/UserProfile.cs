using VetCare.shared.domain;

namespace VetCare.profile.domain.model.aggregates;

public class UserProfile : AggregateRoot
{
    public int Id { get; private set; }
    public int UserId { get; private set; }
    public string Theme { get; private set; }
    public string Language { get; private set; }
    public bool ReceiveNotifications { get; private set; }
    public string? AvatarUrl { get; private set; }

    protected UserProfile() { }

    public UserProfile(int userId, string theme, string language, bool receiveNotifications)
    {
        UserId = userId;
        Theme = theme;
        Language = language;
        ReceiveNotifications = receiveNotifications;
    }

    public void UpdatePreferences(string theme, string language, bool receiveNotifications)
    {
        Theme = theme;
        Language = language;
        ReceiveNotifications = receiveNotifications;
    }

    public void UpdateAvatarUrl(string avatarUrl)
    {
        AvatarUrl = avatarUrl;
    }
}
