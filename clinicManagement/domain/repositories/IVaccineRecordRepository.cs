using VetCare.clinicManagement.domain.model.aggregates;

namespace VetCare.clinicManagement.domain.repositories;

public interface IVaccineRecordRepository
{
    Task<VaccineRecord?> FindByIdAsync(int id);
    Task<IEnumerable<VaccineRecord>> GetAllAsync();
    Task<IEnumerable<VaccineRecord>> FindByPatientIdAsync(int patientId);
    Task AddAsync(VaccineRecord record);
    Task UpdateAsync(VaccineRecord record);
}
