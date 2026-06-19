using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetCare.backoffice.infrastructure.persistence.EFC.context;
using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.application;
using VetCare.backoffice.interfaces.REST.resources;

namespace VetCare.backoffice.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "admin,veterinario")]
public class TicketsController : ControllerBase
{
    private readonly BackofficeContext _context;
    private readonly EconomicsService _economicsService;

    public TicketsController(BackofficeContext context, EconomicsService economicsService)
    {
        _context = context;
        _economicsService = economicsService;
    }

    [HttpGet("consultation/{consultationId}")]
    public async Task<IActionResult> GetByConsultationId(int consultationId)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Items)
            .FirstOrDefaultAsync(t => t.ConsultationId == consultationId);

        if (ticket == null) return NotFound();

        return Ok(ticket);
    }

    [HttpPost("{id}/pay")]
    public async Task<IActionResult> Pay(int id, [FromBody] PayTicketResource request)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == id);
        
        if (ticket == null) return NotFound();
        if (ticket.Status == "Pagado") return BadRequest("Ticket is already paid");

        // Validar método de pago
        if (request.PaymentMethod != "Efectivo" && request.PaymentMethod != "Tarjeta" && 
            request.PaymentMethod != "Banca" && request.PaymentMethod != "Yape")
        {
            return BadRequest("Invalid payment method");
        }

        try
        {
            // Pagar el ticket
            ticket.Pay(request.PaymentMethod);

            // Registrar ingreso económico en la caja
            await _economicsService.CreateEntryAsync(
                type: "Ingreso",
                category: "Servicio Médico",
                amount: ticket.TotalAmount,
                date: DateTime.UtcNow,
                description: $"Cobro de Consulta #{ticket.ConsultationId} - Pago por {request.PaymentMethod}"
            );

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Pago exitoso", TicketId = ticket.Id, Total = ticket.TotalAmount });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
