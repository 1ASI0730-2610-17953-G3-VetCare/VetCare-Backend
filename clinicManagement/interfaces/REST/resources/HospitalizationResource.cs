namespace VetCare.clinicManagement.interfaces.REST.resources;

public record HospitalizationAdmissionResource(
    int Id,
    int PatientId,
    string PatientName,
    string Species,
    int Age,
    string OwnerName,
    string OwnerPhone,
    string Status,
    DateTime AdmissionDate,
    string Diagnosis,
    IReadOnlyList<string> Treatments
);

public record CreateHospitalizationResource(
    int PatientId,
    string Status,
    string Diagnosis,
    DateTime? AdmissionDate,
    IReadOnlyList<string>? Treatments
);

public record UpdateHospitalizationResource(
    string Status,
    string Diagnosis,
    IReadOnlyList<string>? Treatments
);

public record HospitalizationTaskResource(
    int Id,
    int PatientId,
    string PatientName,
    string Status,
    string Title,
    string Description,
    string TaskDate,
    string TaskTime,
    bool Completed
);

public record CreateHospitalizationTaskResource(
    int PatientId,
    string Status,
    string Title,
    string Description,
    DateTime TaskDate,
    string TaskTime
);

public record HospitalizationOverviewResource(
    IReadOnlyList<HospitalizationAdmissionResource> Admissions,
    IReadOnlyList<HospitalizationTaskResource> Tasks
);
