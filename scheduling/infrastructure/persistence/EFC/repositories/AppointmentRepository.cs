using VetCare.scheduling.infrastructure.persistence.EFC.context;
using Microsoft.EntityFrameworkCore;
using VetCare.scheduling.domain.model.aggregates;
using VetCare.scheduling.domain.repositories;

namespace VetCare.scheduling.infrastructure.persistence.EFC.repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly SchedulingContext _context;

    public AppointmentRepository(SchedulingContext context)
    {
        _context = context;
    }

    public async Task<Appointment?> FindByIdAsync(int id)
    {
        return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments.ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> FindByDateAsync(DateTime date)
    {
        return await _context.Appointments.Where(a => a.Date.Date == date.Date).ToListAsync();
    }

    public async Task AddAsync(Appointment appointment)
    {
        await _context.Appointments.AddAsync(appointment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Appointment appointment)
    {
        _context.Appointments.Update(appointment);
        await _context.SaveChangesAsync();
    }
}
