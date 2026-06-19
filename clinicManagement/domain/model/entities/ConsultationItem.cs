namespace VetCare.clinicManagement.domain.model.entities;

public class ConsultationItem
{
    public int Id { get; private set; }
    public int ConsultationId { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    protected ConsultationItem() { }

    public ConsultationItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
