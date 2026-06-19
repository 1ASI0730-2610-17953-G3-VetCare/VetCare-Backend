using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.application;

public class VaccineService
{
    private readonly IVaccineRecordRepository _vaccineRepository;
    private readonly IPatientRepository _patientRepository;

    public VaccineService(
        IVaccineRecordRepository vaccineRepository,
        IPatientRepository patientRepository)
    {
        _vaccineRepository = vaccineRepository;
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<VaccineRecordResource>> GetAllVaccineRecordsAsync()
    {
        var records = await _vaccineRepository.GetAllAsync();
        return await MapRecordsAsync(records);
    }

    public async Task<VaccineRecordResource?> GetVaccineRecordByIdAsync(int id)
    {
        var record = await _vaccineRepository.FindByIdAsync(id);
        if (record == null) return null;

        var mapped = await MapRecordsAsync(new[] { record });
        return mapped.FirstOrDefault();
    }

    public async Task<IEnumerable<VaccineRecordResource>> GetVaccineRecordsByPatientIdAsync(int patientId)
    {
        var records = await _vaccineRepository.FindByPatientIdAsync(patientId);
        return await MapRecordsAsync(records);
    }

    public async Task<VaccineRecord> RegisterVaccineRecordAsync(CreateVaccineRecordResource request)
    {
        var patient = await _patientRepository.FindByIdAsync(request.PatientId);
        if (patient == null)
            throw new ArgumentException($"Patient with ID {request.PatientId} not found.");

        var nextDose = request.NextDose ?? request.LastApplication.AddYears(1);

        var record = new VaccineRecord(
            request.PatientId,
            request.VaccineName,
            request.Disease ?? "-",
            request.LastApplication,
            nextDose,
            request.ProductId);

        await _vaccineRepository.AddAsync(record);
        return record;
    }

    public async Task<VaccineRecord> ApplyVaccineAsync(int id, ApplyVaccineResource request)
    {
        var record = await _vaccineRepository.FindByIdAsync(id);
        if (record == null)
            throw new ArgumentException($"Vaccine record with ID {id} not found.");

        record.ApplyApplication(request.LastApplication, request.NextDose);
        await _vaccineRepository.UpdateAsync(record);
        return record;
    }

    private async Task<IEnumerable<VaccineRecordResource>> MapRecordsAsync(IEnumerable<VaccineRecord> records)
    {
        var recordList = records.ToList();
        if (recordList.Count == 0)
            return Enumerable.Empty<VaccineRecordResource>();

        var patients = (await _patientRepository.GetAllAsync()).ToDictionary(p => p.Id);

        return recordList.Select(record =>
        {
            patients.TryGetValue(record.PatientId, out var patient);
            var patientName = patient?.Name ?? "—";
            var species = patient?.Species.Name ?? "—";

            return new VaccineRecordResource(
                record.Id,
                record.PatientId,
                patientName,
                species,
                record.VaccineName,
                record.Disease,
                record.LastApplication,
                record.NextDose,
                record.ProductId);
        });
    }
}
