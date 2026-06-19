using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.shared.domain;

namespace VetCare.clinicManagement.Domain.Model.Events;

public record ConsultationClosedEvent(Consultation Consultation)
    : DomainEvent(DateTime.UtcNow);