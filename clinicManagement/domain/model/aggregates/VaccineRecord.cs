using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.aggregates;

public class VaccineRecord : AggregateRoot
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public string VaccineName { get; private set; }
    public string Disease { get; private set; }
    public DateTime LastApplication { get; private set; }
    public DateTime NextDose { get; private set; }
    public int? ProductId { get; private set; }

    protected VaccineRecord() { }

    public VaccineRecord(
        int patientId,
        string vaccineName,
        string disease,
        DateTime lastApplication,
        DateTime nextDose,
        int? productId = null)
    {
        if (string.IsNullOrWhiteSpace(vaccineName))
            throw new ArgumentException("Vaccine name is required.");

        PatientId = patientId;
        VaccineName = vaccineName.Trim();
        Disease = string.IsNullOrWhiteSpace(disease) ? "-" : disease.Trim();
        LastApplication = lastApplication.Date;
        NextDose = nextDose.Date;
        ProductId = productId;
    }

    public void ApplyApplication(DateTime lastApplication, DateTime nextDose)
    {
        LastApplication = lastApplication.Date;
        NextDose = nextDose.Date;
    }
}
