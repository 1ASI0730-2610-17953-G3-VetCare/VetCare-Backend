using VetCare.clinicManagement.domain.model.aggregates;

namespace VetCare.clinicManagement.domain.repositories;

public interface IPatientRepository
{
    Task<Patient?> FindByIdAsync(int id);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task<IEnumerable<Patient>> FindByOwnerIdAsync(int ownerId);
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(Patient patient);
}
