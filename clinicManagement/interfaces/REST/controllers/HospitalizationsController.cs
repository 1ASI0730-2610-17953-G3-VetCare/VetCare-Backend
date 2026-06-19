using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.clinicManagement.application;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/hospitalizations")]
[Authorize]
public class HospitalizationsController : ControllerBase
{
    private readonly HospitalizationService _hospitalizationService;
    private readonly ClinicContext _context;

    public HospitalizationsController(HospitalizationService hospitalizationService, ClinicContext context)
    {
        _hospitalizationService = hospitalizationService;
        _context = context;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var overview = await _hospitalizationService.GetOverviewAsync();
        return Ok(overview);
    }

    [HttpPost]
    public async Task<IActionResult> Admit([FromBody] CreateHospitalizationResource request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var admission = await _hospitalizationService.AdmitPatientAsync(request);
            await _context.SaveChangesAsync();

            var created = await _hospitalizationService.GetAdmissionResourceByIdAsync(admission.Id);
            return CreatedAtAction(nameof(GetOverview), created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHospitalizationResource request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _hospitalizationService.UpdateAdmissionAsync(id, request);
            await _context.SaveChangesAsync();

            var updated = await _hospitalizationService.GetAdmissionResourceByIdAsync(id);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/discharge")]
    public async Task<IActionResult> Discharge(int id)
    {
        try
        {
            await _hospitalizationService.DischargePatientAsync(id);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Patient discharged successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("tasks")]
    public async Task<IActionResult> CreateTask([FromBody] CreateHospitalizationTaskResource request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var task = await _hospitalizationService.CreateTaskAsync(request);
            await _context.SaveChangesAsync();

            var created = await _hospitalizationService.GetTaskResourceByIdAsync(task.Id);
            return Ok(created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("tasks/{id}/toggle-complete")]
    public async Task<IActionResult> ToggleTaskComplete(int id)
    {
        try
        {
            await _hospitalizationService.ToggleTaskCompleteAsync(id);
            await _context.SaveChangesAsync();

            var updated = await _hospitalizationService.GetTaskResourceByIdAsync(id);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
