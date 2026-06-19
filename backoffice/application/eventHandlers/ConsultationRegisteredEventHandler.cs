using MediatR;
using VetCare.clinicManagement.Domain.Model.Events;
using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.infrastructure.persistence.EFC.context;

namespace VetCare.backoffice.application.eventHandlers;

public class ConsultationRegisteredEventHandler : INotificationHandler<ConsultationRegisteredEvent>
{
    private readonly BackofficeContext _context;

    public ConsultationRegisteredEventHandler(BackofficeContext context)
    {
        _context = context;
    }

    public async Task Handle(ConsultationRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var consultation = notification.Consultation;

        var ticket = new Ticket(consultation.Id, 30m);

        foreach (var item in consultation.Items)
        {
            ticket.AddItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice);
        }

        await _context.Tickets.AddAsync(ticket, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
