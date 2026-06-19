using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.seed;

public static class ClinicDevelopmentSeeder
{
    public static async Task SeedAsync(ClinicContext context)
    {
        var hasPatients = await context.Patients.AnyAsync();
        if (!hasPatients)
            return;

        await SeedVaccineRecordsAsync(context);
        await SeedHospitalizationAsync(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedVaccineRecordsAsync(ClinicContext context)
    {
        if (await context.VaccineRecords.AnyAsync())
            return;

        var records = new[]
        {
            new VaccineRecord(1, "Antirrábica", "Rabia", new DateTime(2025, 5, 10), new DateTime(2026, 5, 10), 2),
            new VaccineRecord(2, "Séxtuple", "Distemper, Parvovirus, Hepatitis...", new DateTime(2025, 6, 1), new DateTime(2026, 6, 1)),
            new VaccineRecord(3, "Triple Felina", "Panleucopenia, Rinotraqueítis, Calicivirus", new DateTime(2025, 6, 25), new DateTime(2026, 6, 25), 7),
            new VaccineRecord(5, "Parvovirus", "Parvovirosis canina", new DateTime(2025, 8, 15), new DateTime(2026, 8, 15)),
            new VaccineRecord(9, "Leucemia", "Leucemia viral felina", new DateTime(2025, 6, 30), new DateTime(2026, 6, 30))
        };

        foreach (var record in records)
        {
            if (await context.Patients.AnyAsync(p => p.Id == record.PatientId))
                await context.VaccineRecords.AddAsync(record);
        }
    }

    private static async Task SeedHospitalizationAsync(ClinicContext context)
    {
        if (!await context.HospitalizationAdmissions.AnyAsync())
        {
            var admissions = new (int patientId, string status, string diagnosis, DateTime date, string[] treatments)[]
            {
                (1, "Crítico", "Parvovirosis", new DateTime(2026, 5, 9), new[] { "Fluidoterapia IV", "Antibióticos", "Antivirales" }),
                (3, "Estable", "Post-cirugía", new DateTime(2026, 5, 10), new[] { "Analgésicos", "Antibióticos" }),
                (5, "Crítico", "Insuficiencia renal", new DateTime(2026, 5, 11), new[] { "Diálisis", "Soporte nutricional" }),
                (10, "Estable", "Gastroenteritis", new DateTime(2026, 5, 10), new[] { "Hidratación IV", "Dieta blanda" })
            };

            foreach (var item in admissions)
            {
                if (!await context.Patients.AnyAsync(p => p.Id == item.patientId))
                    continue;

                await context.HospitalizationAdmissions.AddAsync(
                    new HospitalizationAdmission(
                        item.patientId,
                        item.status,
                        item.diagnosis,
                        item.date,
                        item.treatments));
            }
        }

        if (await context.HospitalizationTasks.AnyAsync())
            return;

        var today = DateTime.UtcNow.Date;
        var tasks = new (int patientId, string status, string title, string description, string time)[]
        {
            (1, "critico", "Max - Fluidoterapia", "Administrar 500ml solución Ringer", "15:00"),
            (5, "urgente", "Rocky - Control analítica", "Tomar muestra para función renal", "16:00"),
            (3, "en_proceso", "Luna - Revisión post-operatoria", "Verificar estado de sutura — interconsulta pendiente", "18:00"),
            (10, "en_espera", "Toby - Resultados de laboratorio", "Esperando resultados del laboratorio externo", "10:00")
        };

        foreach (var item in tasks)
        {
            if (!await context.Patients.AnyAsync(p => p.Id == item.patientId))
                continue;

            await context.HospitalizationTasks.AddAsync(
                new HospitalizationTask(
                    item.patientId,
                    item.status,
                    item.title,
                    item.description,
                    today,
                    item.time));
        }
    }
}
