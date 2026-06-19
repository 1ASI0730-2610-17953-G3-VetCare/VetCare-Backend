namespace VetCare.clinicManagement.interfaces.REST.resources;

public record VaccineRecordResource(
    int Id,
    int PatientId,
    string PatientName,
    string Species,
    string VaccineName,
    string Disease,
    DateTime LastApplication,
    DateTime NextDose,
    int? ProductId
);

public record CreateVaccineRecordResource(
    int PatientId,
    string VaccineName,
    string? Disease,
    DateTime LastApplication,
    DateTime? NextDose,
    int? ProductId
);

public record ApplyVaccineResource(
    DateTime LastApplication,
    DateTime NextDose
);
