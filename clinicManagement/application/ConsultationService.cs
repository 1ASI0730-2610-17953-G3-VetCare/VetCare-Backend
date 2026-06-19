using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;

namespace VetCare.clinicManagement.application;

public class ConsultationService
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IClientRepository _clientRepository;

    public ConsultationService(
        IConsultationRepository consultationRepository,
        IPatientRepository patientRepository,
        IClientRepository clientRepository)
    {
        _consultationRepository = consultationRepository;
        _patientRepository = patientRepository;
        _clientRepository = clientRepository;
    }

    public async Task<IEnumerable<Consultation>> GetAllConsultationsAsync()
    {
        return await _consultationRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Consultation>> GetConsultationsByPatientIdAsync(int patientId)
    {
        return await _consultationRepository.FindByPatientIdAsync(patientId);
    }

    public async Task<Consultation?> GetConsultationByIdAsync(int id)
    {
        return await _consultationRepository.FindByIdAsync(id);
    }

    public async Task<Consultation> RegisterConsultationAsync(Consultation consultation)
    {
        // Add domain rules if needed, e.g., checking if patient exists
        var patient = await _patientRepository.FindByIdAsync(consultation.PatientId);
        if (patient == null)
            throw new ArgumentException($"Patient with ID {consultation.PatientId} not found.");

        await _consultationRepository.AddAsync(consultation);
        await RecordClientVisitAsync(consultation);
        consultation.MarkRegistered();
        return consultation;
    }

    public async Task CompleteConsultationAsync(Consultation consultation)
    {
        consultation.Complete();
        await RecordClientVisitAsync(consultation);
        await _consultationRepository.UpdateAsync(consultation);
    }

    public async Task UpdateConsultationAsync(Consultation consultation)
    {
        await _consultationRepository.UpdateAsync(consultation);
    }

    private async Task RecordClientVisitAsync(Consultation consultation)
    {
        var patient = await _patientRepository.FindByIdAsync(consultation.PatientId);
        if (patient is null) return;

        var client = await _clientRepository.FindByIdAsync(patient.OwnerId);
        if (client is null) return;

        var visitDate = consultation.Date.Kind == DateTimeKind.Utc
            ? consultation.Date
            : DateTime.SpecifyKind(consultation.Date, DateTimeKind.Utc);

        client.RecordVisit(visitDate);
        await _clientRepository.UpdateAsync(client);
    }
}
