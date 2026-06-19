using VetCare.clinicManagement.domain.model.aggregates;

namespace VetCare.clinicManagement.domain.repositories;

public interface IConsultationRepository
{
    Task<Consultation?> FindByIdAsync(int id);
    Task<IEnumerable<Consultation>> GetAllAsync();
    Task<IEnumerable<Consultation>> FindByPatientIdAsync(int patientId);
    Task AddAsync(Consultation consultation);
    Task UpdateAsync(Consultation consultation);
}
