using VetCare.shared.domain;

namespace VetCare.scheduling.domain.model.events;

public record AppointmentCreatedEvent(
    int AppointmentId, 
    int ClientId, 
    DateTime Date, 
    string Type,
    DateTime OccurredOn
) : DomainEvent(OccurredOn);
