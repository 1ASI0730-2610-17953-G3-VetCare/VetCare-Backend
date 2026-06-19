using VetCare.shared.domain;

namespace VetCare.backoffice.domain.model.aggregates;

public class InventoryTransaction : AggregateRoot
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string Type { get; private set; } // "Ingreso", "Salida", "Ajuste"
    public int QuantityChange { get; private set; } // Puede ser positivo o negativo
    public int PreviousStock { get; private set; }
    public int NewStock { get; private set; }
    public string Reason { get; private set; }
    public DateTime Date { get; private set; }
    public string ResponsibleUser { get; private set; }

    protected InventoryTransaction() { }

    public InventoryTransaction(int productId, string type, int quantityChange, int previousStock, int newStock, string reason, string responsibleUser)
    {
        ProductId = productId;
        Type = type;
        QuantityChange = quantityChange;
        PreviousStock = previousStock;
        NewStock = newStock;
        Reason = reason;
        ResponsibleUser = responsibleUser;
        Date = DateTime.UtcNow;
    }
}
