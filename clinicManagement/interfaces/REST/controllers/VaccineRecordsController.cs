using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.clinicManagement.application;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/vaccine-records")]
[Authorize]
public class VaccineRecordsController : ControllerBase
{
    private readonly VaccineService _vaccineService;
    private readonly ClinicContext _context;

    public VaccineRecordsController(VaccineService vaccineService, ClinicContext context)
    {
        _vaccineService = vaccineService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var records = await _vaccineService.GetAllVaccineRecordsAsync();
        return Ok(records);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var record = await _vaccineService.GetVaccineRecordByIdAsync(id);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatientId(int patientId)
    {
        var records = await _vaccineService.GetVaccineRecordsByPatientIdAsync(patientId);
        return Ok(records);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVaccineRecordResource request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var record = await _vaccineService.RegisterVaccineRecordAsync(request);
            await _context.SaveChangesAsync();

            var created = await _vaccineService.GetVaccineRecordByIdAsync(record.Id);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/apply")]
    public async Task<IActionResult> Apply(int id, [FromBody] ApplyVaccineResource request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _vaccineService.ApplyVaccineAsync(id, request);
            await _context.SaveChangesAsync();

            var updated = await _vaccineService.GetVaccineRecordByIdAsync(id);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
