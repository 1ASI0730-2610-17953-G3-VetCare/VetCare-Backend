using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.repositories;

public class ConsultationRepository : IConsultationRepository
{
    private readonly ClinicContext _context;

    public ConsultationRepository(ClinicContext context)
    {
        _context = context;
    }

    public async Task<Consultation?> FindByIdAsync(int id)
    {
        return await _context.Consultations.FindAsync(id);
    }

    public async Task<IEnumerable<Consultation>> GetAllAsync()
    {
        return await _context.Consultations
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Consultation>> FindByPatientIdAsync(int patientId)
    {
        return await _context.Consultations
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(Consultation consultation)
    {
        await _context.Consultations.AddAsync(consultation);
    }

    public Task UpdateAsync(Consultation consultation)
    {
        _context.Consultations.Update(consultation);
        return Task.CompletedTask;
    }
}
