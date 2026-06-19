using VetCare.shared.domain;

namespace VetCare.shared.persistence.EFC.extensions.eventDispatcher;

public interface IDomainEventDispatcher
{
    Task DispatchEventsAsync(IEnumerable<AggregateRoot> aggregates);
}
