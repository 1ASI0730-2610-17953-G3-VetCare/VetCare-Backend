namespace VetCare.profile.interfaces.REST.resources;

public record ProfileProductivityResource(
    int MonthlyAppointments,
    int PatientsAttended,
    int MonthlyGoalPercent);

public record ProfileDetailResource(
    int Id,
    int UserId,
    string Theme,
    string Language,
    bool ReceiveNotifications,
    string? AvatarUrl,
    ProfileProductivityResource Productivity);
