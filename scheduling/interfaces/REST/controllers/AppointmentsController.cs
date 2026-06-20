using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.scheduling.application;
using VetCare.scheduling.interfaces.REST.resources;

namespace VetCare.scheduling.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/appointments")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentResource request)
    {
        try
        {
            var appointment = await _appointmentService.CreateAppointmentAsync(
                request.PatientId, request.ClientId, request.Date, request.StartTime, request.EndTime, request.Type);
            return Created("", appointment);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/confirm")]
    public async Task<IActionResult> Confirm(int id)
    {
        try
        {
            await _appointmentService.ConfirmAppointmentAsync(id);
            return Ok(new { message = "Appointment confirmed successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        try
        {
            await _appointmentService.CancelAppointmentAsync(id);
            return Ok(new { message = "Appointment cancelled successfully." });
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("by-date")]
    public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
    {
        var appointments = await _appointmentService.GetAppointmentsByDateAsync(date);
        return Ok(appointments);
    }
}
