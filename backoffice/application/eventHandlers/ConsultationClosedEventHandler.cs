using MediatR;
using VetCare.clinicManagement.Domain.Model.Events;
using VetCare.backoffice.application;

namespace VetCare.backoffice.application.eventHandlers;

public class ConsultationClosedEventHandler : INotificationHandler<ConsultationClosedEvent>
{
    private readonly InventoryService _inventoryService;

    public ConsultationClosedEventHandler(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    public async Task Handle(ConsultationClosedEvent notification, CancellationToken cancellationToken)
    {
        var consultation = notification.Consultation;

        foreach (var item in consultation.Items)
        {
            if (item.Quantity > 0)
            {
                await _inventoryService.DeductStockAsync(item.ProductId, item.Quantity);
            }
        }
    }
}
