using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.application;

public class HospitalizationService
{
    private readonly IHospitalizationAdmissionRepository _admissionRepository;
    private readonly IHospitalizationTaskRepository _taskRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IClientRepository _clientRepository;

    public HospitalizationService(
        IHospitalizationAdmissionRepository admissionRepository,
        IHospitalizationTaskRepository taskRepository,
        IPatientRepository patientRepository,
        IClientRepository clientRepository)
    {
        _admissionRepository = admissionRepository;
        _taskRepository = taskRepository;
        _patientRepository = patientRepository;
        _clientRepository = clientRepository;
    }

    public async Task<HospitalizationOverviewResource> GetOverviewAsync()
    {
        var admissions = await _admissionRepository.GetActiveAsync();
        var tasks = await _taskRepository.GetAllAsync();

        return new HospitalizationOverviewResource(
            await MapAdmissionsAsync(admissions),
            await MapTasksAsync(tasks));
    }

    public async Task<HospitalizationAdmission> AdmitPatientAsync(CreateHospitalizationResource request)
    {
        var patient = await _patientRepository.FindByIdAsync(request.PatientId);
        if (patient == null)
            throw new ArgumentException($"Patient with ID {request.PatientId} not found.");

        var admission = new HospitalizationAdmission(
            request.PatientId,
            request.Status,
            request.Diagnosis,
            request.AdmissionDate ?? DateTime.UtcNow,
            request.Treatments ?? Array.Empty<string>());

        await _admissionRepository.AddAsync(admission);

        patient.MarkInTreatment();
        await _patientRepository.UpdateAsync(patient);

        return admission;
    }

    public async Task<HospitalizationAdmission> UpdateAdmissionAsync(int id, UpdateHospitalizationResource request)
    {
        var admission = await _admissionRepository.FindByIdAsync(id);
        if (admission == null || !admission.IsActive)
            throw new ArgumentException($"Active hospitalization with ID {id} not found.");

        admission.Update(request.Status, request.Diagnosis, request.Treatments ?? Array.Empty<string>());
        await _admissionRepository.UpdateAsync(admission);
        return admission;
    }

    public async Task DischargePatientAsync(int id)
    {
        var admission = await _admissionRepository.FindByIdAsync(id);
        if (admission == null || !admission.IsActive)
            throw new ArgumentException($"Active hospitalization with ID {id} not found.");

        admission.Discharge();
        await _admissionRepository.UpdateAsync(admission);
        await _taskRepository.DeleteByPatientIdAsync(admission.PatientId);

        var stillHospitalized = (await _admissionRepository.GetActiveAsync())
            .Any(a => a.PatientId == admission.PatientId);

        if (!stillHospitalized)
        {
            var patient = await _patientRepository.FindByIdAsync(admission.PatientId);
            if (patient != null)
            {
                patient.MarkAsActive();
                await _patientRepository.UpdateAsync(patient);
            }
        }
    }

    public async Task<HospitalizationTask> CreateTaskAsync(CreateHospitalizationTaskResource request)
    {
        var patient = await _patientRepository.FindByIdAsync(request.PatientId);
        if (patient == null)
            throw new ArgumentException($"Patient with ID {request.PatientId} not found.");

        var task = new HospitalizationTask(
            request.PatientId,
            request.Status,
            request.Title,
            request.Description,
            request.TaskDate,
            request.TaskTime);

        await _taskRepository.AddAsync(task);
        return task;
    }

    public async Task<HospitalizationTask> ToggleTaskCompleteAsync(int id)
    {
        var task = await _taskRepository.FindByIdAsync(id);
        if (task == null)
            throw new ArgumentException($"Task with ID {id} not found.");

        task.ToggleComplete();
        await _taskRepository.UpdateAsync(task);
        return task;
    }

    public async Task<HospitalizationAdmissionResource?> GetAdmissionResourceByIdAsync(int id)
    {
        var admission = await _admissionRepository.FindByIdAsync(id);
        if (admission == null) return null;
        return (await MapAdmissionsAsync(new[] { admission })).FirstOrDefault();
    }

    public async Task<HospitalizationTaskResource?> GetTaskResourceByIdAsync(int id)
    {
        var task = await _taskRepository.FindByIdAsync(id);
        if (task == null) return null;
        return (await MapTasksAsync(new[] { task })).FirstOrDefault();
    }

    private async Task<IReadOnlyList<HospitalizationAdmissionResource>> MapAdmissionsAsync(
        IEnumerable<HospitalizationAdmission> admissions)
    {
        var list = admissions.ToList();
        if (list.Count == 0) return Array.Empty<HospitalizationAdmissionResource>();

        var patients = (await _patientRepository.GetAllAsync()).ToDictionary(p => p.Id);
        var clients = (await _clientRepository.GetAllAsync()).ToDictionary(c => c.Id);

        return list.Select(admission =>
        {
            patients.TryGetValue(admission.PatientId, out var patient);
            Client? owner = null;
            if (patient != null)
                clients.TryGetValue(patient.OwnerId, out owner);

            return new HospitalizationAdmissionResource(
                admission.Id,
                admission.PatientId,
                patient?.Name ?? "—",
                patient?.Species.Name ?? "—",
                patient?.Age ?? 0,
                owner?.FullName ?? "—",
                owner?.Phone ?? "—",
                admission.Status,
                admission.AdmissionDate,
                admission.Diagnosis,
                admission.GetTreatments());
        }).ToList();
    }

    private async Task<IReadOnlyList<HospitalizationTaskResource>> MapTasksAsync(
        IEnumerable<HospitalizationTask> tasks)
    {
        var list = tasks.ToList();
        if (list.Count == 0) return Array.Empty<HospitalizationTaskResource>();

        var patients = (await _patientRepository.GetAllAsync()).ToDictionary(p => p.Id);

        return list.Select(task =>
        {
            patients.TryGetValue(task.PatientId, out var patient);
            return new HospitalizationTaskResource(
                task.Id,
                task.PatientId,
                patient?.Name ?? "—",
                task.Status,
                task.Title,
                task.Description,
                task.TaskDate.ToString("yyyy-MM-dd"),
                task.TaskTime,
                task.Completed);
        }).ToList();
    }
}
