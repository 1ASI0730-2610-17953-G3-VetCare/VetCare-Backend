using MediatR;
using VetCare.shared.domain;

namespace VetCare.shared.persistence.EFC.extensions.eventDispatcher;

public class MediatRDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public MediatRDomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task DispatchEventsAsync(IEnumerable<AggregateRoot> aggregates)
    {
        var domainEvents = aggregates
            .SelectMany(a => a.DomainEvents)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            aggregate.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
