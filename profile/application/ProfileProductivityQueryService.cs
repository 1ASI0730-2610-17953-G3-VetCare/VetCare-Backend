using Microsoft.Extensions.Configuration;
using Npgsql;
using VetCare.API.Infrastructure;

namespace VetCare.profile.application;

public class ProfileProductivityQueryService
{
    public const int MonthlyAppointmentGoal = 20;

    private readonly string _connectionString;

    public ProfileProductivityQueryService(IConfiguration configuration)
    {
        _connectionString = DatabaseBootstrap.EnhanceConnectionString(
            configuration.GetConnectionString("DefaultConnection"))
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public async Task<ProfileProductivityStats> GetMonthlyStatsAsync(
        string? doctorFirstName = null,
        string? doctorLastName = null)
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfNextMonth = startOfMonth.AddMonths(1);

        var filterByDoctor = !string.IsNullOrWhiteSpace(doctorFirstName)
            && !string.IsNullOrWhiteSpace(doctorLastName);

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var monthlyAppointments = await CountMonthlyAppointmentsAsync(
            connection, startOfMonth, startOfNextMonth);

        var patientsAttended = await CountPatientsAttendedAsync(
            connection,
            startOfMonth,
            startOfNextMonth,
            filterByDoctor,
            doctorFirstName!.Trim(),
            doctorLastName!.Trim());

        var monthlyGoalPercent = MonthlyAppointmentGoal <= 0
            ? 0
            : Math.Min(100, (int)Math.Round(monthlyAppointments * 100.0 / MonthlyAppointmentGoal));

        return new ProfileProductivityStats
        {
            MonthlyAppointments = monthlyAppointments,
            PatientsAttended = patientsAttended,
            MonthlyGoalPercent = monthlyGoalPercent
        };
    }

    private static async Task<int> CountMonthlyAppointmentsAsync(
        NpgsqlConnection connection,
        DateTime startOfMonth,
        DateTime startOfNextMonth)
    {
        const string query = """
            SELECT COUNT(*)
            FROM appointments
            WHERE CAST(date AS date) >= @startOfMonth
              AND CAST(date AS date) < @startOfNextMonth
              AND LOWER(status) NOT IN ('cancelada')
            """;

        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@startOfMonth", startOfMonth);
        cmd.Parameters.AddWithValue("@startOfNextMonth", startOfNextMonth);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private static async Task<int> CountPatientsAttendedAsync(
        NpgsqlConnection connection,
        DateTime startOfMonth,
        DateTime startOfNextMonth,
        bool filterByDoctor,
        string doctorFirstName,
        string doctorLastName)
    {
        var query = """
            SELECT COUNT(DISTINCT patient_id)
            FROM consultations
            WHERE CAST(date AS date) >= @startOfMonth
              AND CAST(date AS date) < @startOfNextMonth
              AND LOWER(status) IN ('completada', 'cerrada')
            """;

        if (filterByDoctor)
        {
            query += """
                
                  AND doctor_name ILIKE @doctorFirstName
                  AND doctor_name ILIKE @doctorLastName
                """;
        }

        await using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@startOfMonth", startOfMonth);
        cmd.Parameters.AddWithValue("@startOfNextMonth", startOfNextMonth);

        if (filterByDoctor)
        {
            cmd.Parameters.AddWithValue("@doctorFirstName", $"%{doctorFirstName}%");
            cmd.Parameters.AddWithValue("@doctorLastName", $"%{doctorLastName}%");
        }

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }
}

public class ProfileProductivityStats
{
    public int MonthlyAppointments { get; set; }
    public int PatientsAttended { get; set; }
    public int MonthlyGoalPercent { get; set; }
}
