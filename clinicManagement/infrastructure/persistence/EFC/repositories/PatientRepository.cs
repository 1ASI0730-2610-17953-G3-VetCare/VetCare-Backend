using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ClinicContext _context;

    public PatientRepository(ClinicContext context)
    {
        _context = context;
    }

    public async Task<Patient?> FindByIdAsync(int id)
    {
        return await _context.Patients.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Patient>> GetAllAsync()
    {
        return await _context.Patients.ToListAsync();
    }

    public async Task<IEnumerable<Patient>> FindByOwnerIdAsync(int ownerId)
    {
        return await _context.Patients.Where(p => p.OwnerId == ownerId).ToListAsync();
    }

    public async Task AddAsync(Patient patient)
    {
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Patient patient)
    {
        _context.Patients.Update(patient);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Patient patient)
    {
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }
}
