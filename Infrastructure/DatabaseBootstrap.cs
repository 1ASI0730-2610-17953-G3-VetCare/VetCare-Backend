using Microsoft.EntityFrameworkCore;
using Npgsql;
using VetCare.backoffice.infrastructure.persistence.EFC.context;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using VetCare.clinicManagement.infrastructure.persistence.EFC.seed;
using VetCare.communication.infrastructure.persistence.EFC.context;
using VetCare.iam.infrastructure.persistence.EFC.context;
using VetCare.profile.infrastructure.persistence.EFC.context;
using VetCare.scheduling.infrastructure.persistence.EFC.context;

namespace VetCare.API.Infrastructure;

public static class DatabaseBootstrap
{
    private const int MaxAttempts = 4;

    public static string EnhanceConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return string.Empty;

        var builder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Timeout = 60,
            CommandTimeout = 60,
            KeepAlive = 30,
            SslMode = SslMode.Require,
            TrustServerCertificate = true
        };

        return builder.ConnectionString;
    }

    public static async Task ApplyMigrationsAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        await MigrateWithRetryAsync(provider.GetRequiredService<IamContext>(), nameof(IamContext));

        var clinicContext = provider.GetRequiredService<ClinicContext>();
        await MigrateWithRetryAsync(clinicContext, nameof(ClinicContext));
        await ClinicDevelopmentSeeder.SeedAsync(clinicContext);

        await MigrateWithRetryAsync(provider.GetRequiredService<SchedulingContext>(), nameof(SchedulingContext));
        await MigrateWithRetryAsync(provider.GetRequiredService<BackofficeContext>(), nameof(BackofficeContext));
        await MigrateWithRetryAsync(provider.GetRequiredService<ProfileContext>(), nameof(ProfileContext));
        await MigrateWithRetryAsync(provider.GetRequiredService<CommunicationContext>(), nameof(CommunicationContext));
    }

    private static async Task MigrateWithRetryAsync(DbContext context, string contextName)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                await context.Database.MigrateAsync();
                Console.WriteLine($"[DB] {contextName}: migrations up to date.");
                return;
            }
            catch (Exception ex) when (attempt < MaxAttempts && IsTransient(ex))
            {
                var delay = TimeSpan.FromSeconds(5 * attempt);
                Console.WriteLine(
                    $"[DB] {contextName}: connection failed (attempt {attempt}/{MaxAttempts}). " +
                    $"Retrying in {delay.TotalSeconds}s...");
                Console.WriteLine($"       {ex.InnerException?.Message ?? ex.Message}");
                await Task.Delay(delay);
            }
        }
    }

    private static bool IsTransient(Exception ex)
    {
        for (var current = ex; current is not null; current = current.InnerException)
        {
            if (current is TimeoutException)
                return true;

            if (current is NpgsqlException npgsql &&
                (npgsql.IsTransient || npgsql.InnerException is TimeoutException))
                return true;
        }

        return false;
    }
}
