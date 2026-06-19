using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.repositories;

public class HospitalizationAdmissionRepository : IHospitalizationAdmissionRepository
{
    private readonly ClinicContext _context;

    public HospitalizationAdmissionRepository(ClinicContext context)
    {
        _context = context;
    }

    public async Task<HospitalizationAdmission?> FindByIdAsync(int id)
    {
        return await _context.HospitalizationAdmissions.FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<HospitalizationAdmission>> GetActiveAsync()
    {
        return await _context.HospitalizationAdmissions
            .Where(a => a.DischargedAt == null)
            .OrderByDescending(a => a.AdmissionDate)
            .ToListAsync();
    }

    public async Task AddAsync(HospitalizationAdmission admission)
    {
        await _context.HospitalizationAdmissions.AddAsync(admission);
    }

    public Task UpdateAsync(HospitalizationAdmission admission)
    {
        _context.HospitalizationAdmissions.Update(admission);
        return Task.CompletedTask;
    }
}

public class HospitalizationTaskRepository : IHospitalizationTaskRepository
{
    private readonly ClinicContext _context;

    public HospitalizationTaskRepository(ClinicContext context)
    {
        _context = context;
    }

    public async Task<HospitalizationTask?> FindByIdAsync(int id)
    {
        return await _context.HospitalizationTasks.FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<HospitalizationTask>> GetAllAsync()
    {
        return await _context.HospitalizationTasks
            .OrderByDescending(t => t.TaskDate)
            .ThenBy(t => t.TaskTime)
            .ToListAsync();
    }

    public async Task AddAsync(HospitalizationTask task)
    {
        await _context.HospitalizationTasks.AddAsync(task);
    }

    public Task UpdateAsync(HospitalizationTask task)
    {
        _context.HospitalizationTasks.Update(task);
        return Task.CompletedTask;
    }

    public async Task DeleteByPatientIdAsync(int patientId)
    {
        var tasks = await _context.HospitalizationTasks
            .Where(t => t.PatientId == patientId)
            .ToListAsync();

        _context.HospitalizationTasks.RemoveRange(tasks);
    }
}
