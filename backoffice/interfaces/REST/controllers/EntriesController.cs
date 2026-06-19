using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.backoffice.application;
using VetCare.backoffice.interfaces.REST.resources;

namespace VetCare.backoffice.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/entries")]
[Authorize(Roles = "admin")]
public class EntriesController : ControllerBase
{
    private readonly EconomicsService _economicsService;

    public EntriesController(EconomicsService economicsService)
    {
        _economicsService = economicsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _economicsService.GetAllEntriesAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntryResource request)
    {
        try
        {
            var entry = await _economicsService.CreateEntryAsync(request.Type, request.Category, request.Amount, request.Date, request.Description);
            return Created("", entry);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
