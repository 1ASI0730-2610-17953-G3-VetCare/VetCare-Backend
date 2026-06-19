using VetCare.backoffice.domain.model.aggregates;
using VetCare.shared.domain;

namespace VetCare.backoffice.Domain.Model.Events;

public record StockDeductedEvent(
    Product Product, int DeductedQuantity, int PreviousStock, int NewStock)
    : DomainEvent(DateTime.UtcNow);