using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.repositories;

public class VaccineRecordRepository : IVaccineRecordRepository
{
    private readonly ClinicContext _context;

    public VaccineRecordRepository(ClinicContext context)
    {
        _context = context;
    }

    public async Task<VaccineRecord?> FindByIdAsync(int id)
    {
        return await _context.VaccineRecords.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<VaccineRecord>> GetAllAsync()
    {
        return await _context.VaccineRecords
            .OrderByDescending(r => r.NextDose)
            .ToListAsync();
    }

    public async Task<IEnumerable<VaccineRecord>> FindByPatientIdAsync(int patientId)
    {
        return await _context.VaccineRecords
            .Where(r => r.PatientId == patientId)
            .OrderByDescending(r => r.LastApplication)
            .ToListAsync();
    }

    public async Task AddAsync(VaccineRecord record)
    {
        await _context.VaccineRecords.AddAsync(record);
    }

    public Task UpdateAsync(VaccineRecord record)
    {
        _context.VaccineRecords.Update(record);
        return Task.CompletedTask;
    }
}
