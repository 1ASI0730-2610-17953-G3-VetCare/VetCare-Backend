using VetCare.shared.domain;
using VetCare.scheduling.domain.model.valueobjects;
using VetCare.scheduling.domain.model.events;

namespace VetCare.scheduling.domain.model.aggregates;

public class Appointment : AggregateRoot
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public int ClientId { get; private set; }
    public DateTime Date { get; private set; }
    public TimeSlot TimeSlot { get; private set; }
    public string Status { get; private set; }
    public string Type { get; private set; }

    protected Appointment() { } // EF Core

    public Appointment(int patientId, int clientId, DateTime date, TimeSlot timeSlot, string type)
    {
        PatientId = patientId;
        ClientId = clientId;
        Date = date.Date; // Store only the date part
        TimeSlot = timeSlot;
        Type = type;
        Status = "Pendiente";

        AddDomainEvent(new AppointmentCreatedEvent(
            0, // ID is generated later
            clientId, 
            date, 
            type,
            DateTime.UtcNow
        ));
    }

    public void Confirm()
    {
        Status = "Confirmada";
    }
    
    public void Cancel()
    {
        Status = "Cancelada";
    }
}
