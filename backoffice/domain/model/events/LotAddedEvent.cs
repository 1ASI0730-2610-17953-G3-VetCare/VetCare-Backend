using VetCare.backoffice.domain.model.aggregates;
using VetCare.shared.domain;

namespace VetCare.backoffice.Domain.Model.Events;

public record LotAddedEvent(
    Product Product, int AddedQuantity, int PreviousStock, int NewStock, string LotNumber)
    : DomainEvent(DateTime.UtcNow);