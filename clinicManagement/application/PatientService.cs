using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.model.valueobjects;
using VetCare.clinicManagement.domain.repositories;

namespace VetCare.clinicManagement.application;

public class PatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IVaccineRecordRepository _vaccineRecordRepository;
    private readonly IHospitalizationAdmissionRepository _hospitalizationAdmissionRepository;
    private readonly IHospitalizationTaskRepository _hospitalizationTaskRepository;

    public PatientService(
        IPatientRepository patientRepository,
        IClientRepository clientRepository,
        IConsultationRepository consultationRepository,
        IVaccineRecordRepository vaccineRecordRepository,
        IHospitalizationAdmissionRepository hospitalizationAdmissionRepository,
        IHospitalizationTaskRepository hospitalizationTaskRepository)
    {
        _patientRepository = patientRepository;
        _clientRepository = clientRepository;
        _consultationRepository = consultationRepository;
        _vaccineRecordRepository = vaccineRecordRepository;
        _hospitalizationAdmissionRepository = hospitalizationAdmissionRepository;
        _hospitalizationTaskRepository = hospitalizationTaskRepository;
    }

    public async Task<Patient> CreatePatientAsync(string name, string speciesName, int age, double weightValue, int ownerId)
    {
        var owner = await _clientRepository.FindByIdAsync(ownerId);
        if (owner == null)
            throw new ArgumentException("Owner not found.");

        var code = "PAT" + DateTime.UtcNow.Ticks.ToString().Substring(10);
        var species = new Species(speciesName);
        var weight = new Weight(weightValue);

        var patient = new Patient(code, name, species, age, weight, ownerId);
        await _patientRepository.AddAsync(patient);
        return patient;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        return await _patientRepository.GetAllAsync();
    }

    public async Task<Patient> UpdatePatientClinicalProfileAsync(int id, int age, double weightValue)
    {
        var patient = await _patientRepository.FindByIdAsync(id);
        if (patient == null)
            throw new ArgumentException("Patient not found.");

        patient.UpdateClinicalProfile(age, weightValue);
        await _patientRepository.UpdateAsync(patient);
        return patient;
    }

    public async Task DeletePatientAsync(int id)
    {
        var patient = await _patientRepository.FindByIdAsync(id);
        if (patient == null)
            throw new ArgumentException("Patient not found.");

        if ((await _consultationRepository.FindByPatientIdAsync(id)).Any())
            throw new InvalidOperationException("PatientHasClinicalRecords");

        if ((await _vaccineRecordRepository.FindByPatientIdAsync(id)).Any())
            throw new InvalidOperationException("PatientHasClinicalRecords");

        var activeAdmissions = await _hospitalizationAdmissionRepository.GetActiveAsync();
        if (activeAdmissions.Any(admission => admission.PatientId == id))
            throw new InvalidOperationException("PatientHasClinicalRecords");

        var tasks = await _hospitalizationTaskRepository.GetAllAsync();
        if (tasks.Any(task => task.PatientId == id))
            throw new InvalidOperationException("PatientHasClinicalRecords");

        await _patientRepository.DeleteAsync(patient);
    }
}
