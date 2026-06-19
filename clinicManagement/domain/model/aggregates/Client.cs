using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.aggregates;

public class Client : AggregateRoot
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string FullName { get; private set; }
    public string DocumentId { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string Address { get; private set; }
    public string Status { get; private set; }
    public DateTime? LastVisitAt { get; private set; }

    private const int InactivityMonths = 12;

    protected Client() { } // EF Core

    public Client(string code, string fullName, string documentId, string phone, string email, string address)
    {
        Code = code;
        FullName = fullName;
        DocumentId = documentId;
        Phone = phone;
        Email = email;
        Address = address;
        Status = "Activo";
    }

    public void RecordVisit(DateTime? visitDate = null)
    {
        var visit = visitDate ?? DateTime.UtcNow;
        if (LastVisitAt is null || visit > LastVisitAt)
            LastVisitAt = visit;

        ApplyInactivityRule();
    }

    public void ApplyInactivityRule()
    {
        if (LastVisitAt is null)
        {
            Status = "Activo";
            return;
        }

        Status = LastVisitAt.Value < DateTime.UtcNow.AddMonths(-InactivityMonths)
            ? "Inactivo"
            : "Activo";
    }
}
