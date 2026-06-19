namespace VetCare.scheduling.interfaces.REST.resources;

public record CreateAppointmentResource(int PatientId, int ClientId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime, string Type);
