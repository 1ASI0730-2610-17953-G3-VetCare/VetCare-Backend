using VetCare.shared.domain;

namespace VetCare.backoffice.domain.model.aggregates;

public class Ticket : AggregateRoot
{
    public int Id { get; private set; }
    public int ConsultationId { get; private set; }
    public decimal SubTotal { get; private set; }
    public decimal ConsultationBasePrice { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } // Pendiente, Pagado
    public string? PaymentMethod { get; private set; } // Efectivo, Tarjeta, Banca, Yape
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }

    // Representa los items que se están cobrando
    private readonly List<TicketItem> _items = new();
    public IReadOnlyCollection<TicketItem> Items => _items.AsReadOnly();

    protected Ticket() { }

    public Ticket(int consultationId, decimal consultationBasePrice)
    {
        ConsultationId = consultationId;
        ConsultationBasePrice = consultationBasePrice;
        Status = "Pendiente";
        CreatedAt = DateTime.UtcNow;
        CalculateTotal();
    }

    public void AddItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        _items.Add(new TicketItem(productId, productName, quantity, unitPrice));
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        SubTotal = _items.Sum(i => i.Quantity * i.UnitPrice);
        TotalAmount = SubTotal + ConsultationBasePrice;
    }

    public void Pay(string paymentMethod)
    {
        if (Status == "Pagado") throw new InvalidOperationException("El ticket ya está pagado.");
        
        PaymentMethod = paymentMethod;
        Status = "Pagado";
        PaidAt = DateTime.UtcNow;
    }
}

public class TicketItem
{
    public int Id { get; private set; }
    public int TicketId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    protected TicketItem() { }

    public TicketItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
