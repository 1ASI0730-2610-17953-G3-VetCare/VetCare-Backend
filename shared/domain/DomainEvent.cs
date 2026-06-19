using MediatR;

namespace VetCare.shared.domain;

public abstract record DomainEvent(DateTime OccurredOn) : INotification;
