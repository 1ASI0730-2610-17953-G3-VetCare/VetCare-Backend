using VetCare.scheduling.domain.model.aggregates;

namespace VetCare.scheduling.domain.repositories;

public interface IAppointmentRepository
{
    Task<Appointment?> FindByIdAsync(int id);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<IEnumerable<Appointment>> FindByDateAsync(DateTime date);
    Task AddAsync(Appointment appointment);
    Task UpdateAsync(Appointment appointment);
}
