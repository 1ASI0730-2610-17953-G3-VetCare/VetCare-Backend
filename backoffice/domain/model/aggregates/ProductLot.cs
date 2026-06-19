namespace VetCare.backoffice.Domain.Model.Entities;

public class ProductLot
{
    public int Id { get; private set; }
    public string LotNumber { get; private set; }
    public int InitialQuantity { get; private set; }
    public int CurrentQuantity { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public int ProductId { get; private set; }

    protected ProductLot() { }

    public ProductLot(string lotNumber, int initialQuantity, DateTime expiryDate)
    {
        LotNumber = lotNumber;
        InitialQuantity = initialQuantity;
        CurrentQuantity = initialQuantity;
        ExpiryDate = expiryDate;
    }

    public void DeductQuantity(int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative");
        if (CurrentQuantity < quantity) throw new InvalidOperationException($"Insufficient quantity in lot {LotNumber}. Available: {CurrentQuantity}, Requested: {quantity}");
        CurrentQuantity -= quantity;
    }
}
