using VetCare.shared.domain;

namespace VetCare.backoffice.domain.model.aggregates;

public class Supplier : AggregateRoot
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Contact { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }
    public string Status { get; private set; }

    protected Supplier() { }

    public Supplier(string name, string contact, string phone, string email)
    {
        Name = name;
        Contact = contact;
        Phone = phone;
        Email = email;
        Status = "Activo";
    }
}
