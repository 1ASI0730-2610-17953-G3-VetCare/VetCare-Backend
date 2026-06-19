using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using VetCare.API.Infrastructure;

namespace VetCare.dashboard.application;

public class DashboardQueryService
{
    private readonly string _connectionString;

    public DashboardQueryService(IConfiguration configuration)
    {
        _connectionString = DatabaseBootstrap.EnhanceConnectionString(
            configuration.GetConnectionString("DefaultConnection"))
            ?? throw new ArgumentNullException("Connection string not found");
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var summary = new DashboardSummaryDto();
        var today = DateTime.UtcNow.Date;

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // 1. Get today's appointments count
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM appointments WHERE date = @date", connection))
        {
            cmd.Parameters.AddWithValue("@date", today);
            summary.TodaysAppointments = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        // 2. Get total patients
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM patients", connection))
        {
            summary.TotalPatients = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        // 3. Get low stock products count
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM products WHERE stock <= min_stock", connection))
        {
            summary.LowStockProductsCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        // 4. Get this month's revenue (sum of Ingreso entries)
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(amount), 0) FROM entries WHERE type = 'Ingreso' AND date >= @startOfMonth", connection))
        {
            cmd.Parameters.AddWithValue("@startOfMonth", startOfMonth);
            summary.ThisMonthRevenue = Convert.ToDecimal(await cmd.ExecuteScalarAsync());
        }

        return summary;
    }

    public async Task<AdminDashboardSummaryDto> GetAdminSummaryAsync()
    {
        var summary = new AdminDashboardSummaryDto();
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var in30Days = today.AddDays(30);

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // 1. Today Revenue
        using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(amount), 0) FROM entries WHERE type = 'Ingreso' AND CAST(date AS date) = @today", connection))
        {
            cmd.Parameters.AddWithValue("@today", today);
            summary.TodayRevenue = Convert.ToDecimal(await cmd.ExecuteScalarAsync());
        }

        // 2. Month Revenue
        using (var cmd = new NpgsqlCommand("SELECT COALESCE(SUM(amount), 0) FROM entries WHERE type = 'Ingreso' AND CAST(date AS date) >= @startOfMonth", connection))
        {
            cmd.Parameters.AddWithValue("@startOfMonth", startOfMonth);
            summary.MonthRevenue = Convert.ToDecimal(await cmd.ExecuteScalarAsync());
        }

        // 3. Today Appointments
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM appointments WHERE CAST(date AS date) = @today", connection))
        {
            cmd.Parameters.AddWithValue("@today", today);
            summary.TodayAppointments = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        // 4. Expiring Lots Count
        using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM product_lots WHERE CAST(expiry_date AS date) <= @in30Days AND current_quantity > 0", connection))
        {
            cmd.Parameters.AddWithValue("@in30Days", in30Days);
            summary.ExpiringLotsCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        }

        return summary;
    }

    public async Task<IEnumerable<RevenueDayDto>> GetRevenueChartAsync()
    {
        var result = new List<RevenueDayDto>();
        var date30DaysAgo = DateTime.UtcNow.Date.AddDays(-30);

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT CAST(date AS date) as day, COALESCE(SUM(amount), 0) as total 
            FROM entries 
            WHERE type = 'Ingreso' AND CAST(date AS date) >= @date30DaysAgo 
            GROUP BY CAST(date AS date) 
            ORDER BY day";

        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@date30DaysAgo", date30DaysAgo);
        
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new RevenueDayDto
            {
                Date = reader.GetDateTime(0).ToString("yyyy-MM-dd"),
                Amount = reader.GetDecimal(1)
            });
        }
        return result;
    }

    public async Task<IEnumerable<TopProductDto>> GetTopProductsAsync()
    {
        var result = new List<TopProductDto>();

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT product_name, SUM(quantity) as total_qty 
            FROM ticket_items 
            GROUP BY product_name 
            ORDER BY total_qty DESC 
            LIMIT 5";

        using var cmd = new NpgsqlCommand(query, connection);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new TopProductDto
            {
                Name = reader.GetString(0),
                TotalConsumed = Convert.ToInt32(reader.GetDecimal(1)) // SUM might return numeric
            });
        }
        return result;
    }

    public async Task<IEnumerable<ExpiringLotDto>> GetExpiringLotsAsync()
    {
        var result = new List<ExpiringLotDto>();
        var today = DateTime.UtcNow.Date;
        var in30Days = today.AddDays(30);

        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT p.name, pl.lot_number, pl.expiry_date, pl.current_quantity 
            FROM product_lots pl 
            JOIN products p ON p.id = pl.product_id 
            WHERE CAST(pl.expiry_date AS date) BETWEEN @today AND @in30days AND pl.current_quantity > 0 
            ORDER BY pl.expiry_date";

        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@today", today);
        cmd.Parameters.AddWithValue("@in30days", in30Days);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new ExpiringLotDto
            {
                ProductName = reader.GetString(0),
                LotNumber = reader.GetString(1),
                ExpiryDate = reader.GetDateTime(2),
                CurrentQuantity = reader.GetInt32(3)
            });
        }
        return result;
    }
}

public class DashboardSummaryDto
{
    public int TodaysAppointments { get; set; }
    public int TotalPatients { get; set; }
    public int LowStockProductsCount { get; set; }
    public decimal ThisMonthRevenue { get; set; }
}

public class AdminDashboardSummaryDto 
{
    public decimal TodayRevenue { get; set; }
    public decimal MonthRevenue { get; set; }
    public int TodayAppointments { get; set; }
    public int ExpiringLotsCount { get; set; }
}

public class RevenueDayDto 
{ 
    public string Date { get; set; } = string.Empty;
    public decimal Amount { get; set; } 
}

public class TopProductDto 
{ 
    public string Name { get; set; } = string.Empty;
    public int TotalConsumed { get; set; } 
}

public class ExpiringLotDto 
{ 
    public string ProductName { get; set; } = string.Empty;
    public string LotNumber { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; } 
    public int CurrentQuantity { get; set; } 
}
