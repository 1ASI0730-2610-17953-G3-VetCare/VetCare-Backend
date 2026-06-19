using VetCare.clinicManagement.domain.model.aggregates;

namespace VetCare.clinicManagement.domain.repositories;

public interface IHospitalizationAdmissionRepository
{
    Task<HospitalizationAdmission?> FindByIdAsync(int id);
    Task<IEnumerable<HospitalizationAdmission>> GetActiveAsync();
    Task AddAsync(HospitalizationAdmission admission);
    Task UpdateAsync(HospitalizationAdmission admission);
}

public interface IHospitalizationTaskRepository
{
    Task<HospitalizationTask?> FindByIdAsync(int id);
    Task<IEnumerable<HospitalizationTask>> GetAllAsync();
    Task AddAsync(HospitalizationTask task);
    Task UpdateAsync(HospitalizationTask task);
    Task DeleteByPatientIdAsync(int patientId);
}
