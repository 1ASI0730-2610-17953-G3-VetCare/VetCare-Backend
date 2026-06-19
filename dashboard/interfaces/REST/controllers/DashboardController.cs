using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.dashboard.application;

namespace VetCare.dashboard.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardQueryService _dashboardQueryService;

    public DashboardController(DashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _dashboardQueryService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("admin-summary")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAdminSummary()
    {
        var summary = await _dashboardQueryService.GetAdminSummaryAsync();
        return Ok(summary);
    }

    [HttpGet("revenue-chart")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetRevenueChart()
    {
        var chart = await _dashboardQueryService.GetRevenueChartAsync();
        return Ok(chart);
    }

    [HttpGet("top-products")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetTopProducts()
    {
        var products = await _dashboardQueryService.GetTopProductsAsync();
        return Ok(products);
    }

    [HttpGet("expiring-lots")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetExpiringLots()
    {
        var lots = await _dashboardQueryService.GetExpiringLotsAsync();
        return Ok(lots);
    }
}
