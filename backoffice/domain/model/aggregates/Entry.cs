using VetCare.shared.domain;

namespace VetCare.backoffice.domain.model.aggregates;

public class Entry : AggregateRoot
{
    public int Id { get; private set; }
    public string Type { get; private set; } // "Ingreso" or "Egreso"
    public string Category { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }

    protected Entry() { }

    public Entry(string type, string category, decimal amount, DateTime date, string description)
    {
        if (type != "Ingreso" && type != "Egreso")
            throw new ArgumentException("Type must be Ingreso or Egreso.");
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");
            
        Type = type;
        Category = category;
        Amount = amount;
        Date = date;
        Description = description;
    }
}
