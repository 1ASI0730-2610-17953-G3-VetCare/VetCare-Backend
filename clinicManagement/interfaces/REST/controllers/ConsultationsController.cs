using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.clinicManagement.application;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.shared.persistence.EFC.extensions.eventDispatcher;
using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ConsultationsController : ControllerBase
{
    private readonly ConsultationService _consultationService;
    private readonly ClinicContext _context;
    private readonly IDomainEventDispatcher _dispatcher;

    public ConsultationsController(
        ConsultationService consultationService, 
        ClinicContext context,
        IDomainEventDispatcher dispatcher)
    {
        _consultationService = consultationService;
        _context = context;
        _dispatcher = dispatcher;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var consultations = await _consultationService.GetAllConsultationsAsync();
        return Ok(consultations);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var consultation = await _consultationService.GetConsultationByIdAsync(id);
        if (consultation == null) return NotFound();
        return Ok(consultation);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetByPatientId(int patientId)
    {
        var consultations = await _consultationService.GetConsultationsByPatientIdAsync(patientId);
        return Ok(consultations);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConsultationResource request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var consultation = new Consultation(
            request.PatientId,
            request.DoctorName ?? "Dr. Vet", // Fallback if not provided
            request.Type,
            request.Status,
            request.Subjective,
            request.Objective,
            request.Analysis,
            request.Plan,
            request.Temperature,
            request.HeartRate,
            request.Weight,
            request.BodyCondition,
            request.Date == default ? DateTime.UtcNow : request.Date
        );

        if (request.Items != null)
        {
            foreach (var item in request.Items)
            {
                consultation.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
            }
        }

        try
        {
            await _consultationService.RegisterConsultationAsync(consultation);
            await _context.SaveChangesAsync();
            await _dispatcher.DispatchEventsAsync(new[] { consultation });
            return CreatedAtAction(nameof(GetById), new { id = consultation.Id }, consultation);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var consultation = await _consultationService.GetConsultationByIdAsync(id);
        if (consultation == null) return NotFound();

        await _consultationService.CompleteConsultationAsync(consultation);
        
        // Guardaremos en base de datos primero
        await _context.SaveChangesAsync();

        // El Dispatcher de MediatR necesita procesar los eventos de dominio (como crear ticket)
        await _dispatcher.DispatchEventsAsync(new[] { consultation });

        return Ok(consultation);
    }
}
