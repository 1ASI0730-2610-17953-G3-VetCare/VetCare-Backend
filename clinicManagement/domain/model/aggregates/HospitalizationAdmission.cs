using System.Text.Json;
using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.aggregates;

public class HospitalizationAdmission : AggregateRoot
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public string Status { get; private set; }
    public string Diagnosis { get; private set; }
    public DateTime AdmissionDate { get; private set; }
    public string TreatmentsJson { get; private set; }
    public DateTime? DischargedAt { get; private set; }

    protected HospitalizationAdmission()
    {
        Status = "Estable";
        Diagnosis = string.Empty;
        TreatmentsJson = "[]";
    }

    public HospitalizationAdmission(
        int patientId,
        string status,
        string diagnosis,
        DateTime admissionDate,
        IEnumerable<string> treatments)
    {
        PatientId = patientId;
        Status = status;
        Diagnosis = diagnosis;
        AdmissionDate = admissionDate.Date;
        SetTreatments(treatments);
    }

    public bool IsActive => DischargedAt == null;

    public IReadOnlyList<string> GetTreatments() =>
        JsonSerializer.Deserialize<List<string>>(TreatmentsJson) ?? new List<string>();

    public void Update(string status, string diagnosis, IEnumerable<string> treatments)
    {
        Status = status;
        Diagnosis = diagnosis;
        SetTreatments(treatments);
    }

    public void Discharge()
    {
        DischargedAt = DateTime.UtcNow;
    }

    private void SetTreatments(IEnumerable<string> treatments)
    {
        TreatmentsJson = JsonSerializer.Serialize(
            treatments.Where(t => !string.IsNullOrWhiteSpace(t)).ToList());
    }
}
