using MediatR;
using VetCare.backoffice.Domain.Model.Events;
using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.infrastructure.persistence.EFC.context;

namespace VetCare.backoffice.application.eventHandlers;

public class InventoryTransactionEventHandlers : 
    INotificationHandler<StockDeductedEvent>,
    INotificationHandler<LotAddedEvent>
{
    private readonly BackofficeContext _context;

    public InventoryTransactionEventHandlers(BackofficeContext context)
    {
        _context = context;
    }

    public async Task Handle(StockDeductedEvent notification, CancellationToken cancellationToken)
    {
        // El responsable puede ser pasado a través del contexto de autenticación o en el evento.
        // Como solución temporal, registramos "Sistema".
        var transaction = new InventoryTransaction(
            productId: notification.Product.Id,
            type: "Salida",
            quantityChange: -notification.DeductedQuantity,
            previousStock: notification.PreviousStock,
            newStock: notification.NewStock,
            reason: "Deducción de Stock (Consumo o Venta)",
            responsibleUser: "Sistema"
        );

        await _context.InventoryTransactions.AddAsync(transaction, cancellationToken);
        // Note: No llamamos a _context.SaveChangesAsync() aquí porque MediatR despacha esto 
        // ANTES o DURANTE la transacción original en el DbContext, o podemos depender de que 
        // el creador llame a SaveChangesAsync(). Si el dispatcher es invocado DESPUES del save,
        // necesitamos un save aquí. Revisando MediatRDomainEventDispatcher, se ejecuta *después*
        // o *durante* dependiendo de la integración. Vamos a asegurar guardando:
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(LotAddedEvent notification, CancellationToken cancellationToken)
    {
        var transaction = new InventoryTransaction(
            productId: notification.Product.Id,
            type: "Ingreso",
            quantityChange: notification.AddedQuantity,
            previousStock: notification.PreviousStock,
            newStock: notification.NewStock,
            reason: $"Registro de Lote #{notification.LotNumber}",
            responsibleUser: "Sistema"
        );

        await _context.InventoryTransactions.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
