using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.shared.domain;

namespace VetCare.clinicManagement.Domain.Model.Events;

public record ConsultationRegisteredEvent(Consultation Consultation)
    : DomainEvent(DateTime.UtcNow);
