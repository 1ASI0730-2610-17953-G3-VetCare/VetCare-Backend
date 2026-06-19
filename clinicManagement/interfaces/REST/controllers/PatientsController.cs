using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.clinicManagement.application;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/patients")]
[Authorize]
public class PatientsController : ControllerBase
{
    private readonly PatientService _patientService;

    public PatientsController(PatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var patients = await _patientService.GetAllPatientsAsync();
        return Ok(patients);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePatientResource request)
    {
        try
        {
            var patient = await _patientService.CreatePatientAsync(
                request.Name, request.Species, request.Age, request.Weight, request.OwnerId);
            return Created("", patient);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateClinicalProfile(int id, [FromBody] UpdatePatientResource request)
    {
        try
        {
            var patient = await _patientService.UpdatePatientClinicalProfileAsync(
                id, request.Age, request.Weight);
            return Ok(patient);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
