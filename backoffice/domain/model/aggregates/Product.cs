using VetCare.shared.domain;

using VetCare.backoffice.Domain.Model.Entities;

namespace VetCare.backoffice.domain.model.aggregates;

public class Product : AggregateRoot
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Category { get; private set; }
    public int Stock { get; private set; }
    public int MinStock { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; }

    private readonly List<ProductLot> _lots = new();
    public IReadOnlyCollection<ProductLot> Lots => _lots.AsReadOnly();

    protected Product() { }

    public Product(string code, string name, string category, int stock, int minStock, decimal price)
    {
        Code = code;
        Name = name;
        Category = category;
        Stock = stock;
        MinStock = minStock;
        Price = price;
        Status = "Activo";
    }

    public void AddLot(ProductLot lot)
    {
        var previousStock = Stock;
        _lots.Add(lot);
        UpdateStockFromLots();
        AddDomainEvent(new VetCare.backoffice.Domain.Model.Events.LotAddedEvent(this, lot.CurrentQuantity, previousStock, Stock, lot.LotNumber));
    }

    public void DeductStock(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity to deduct must be positive.");
        if (Stock < quantity) throw new InvalidOperationException($"Not enough stock for product {Name}. Requested: {quantity}, Available: {Stock}");

        var previousStock = Stock;
        var remainingToDeduct = quantity;

        // FIFO: Order by expiry date (oldest first)
        var availableLots = _lots
            .Where(l => l.CurrentQuantity > 0)
            .OrderBy(l => l.ExpiryDate)
            .ToList();

        foreach (var lot in availableLots)
        {
            if (remainingToDeduct == 0) break;

            var deduction = Math.Min(lot.CurrentQuantity, remainingToDeduct);
            lot.DeductQuantity(deduction);
            remainingToDeduct -= deduction;
        }

        UpdateStockFromLots();
        AddDomainEvent(new VetCare.backoffice.Domain.Model.Events.StockDeductedEvent(this, quantity, previousStock, Stock));
    }

    public void UpdateStock(int newStock)
    {
        if (newStock < 0) throw new ArgumentException("Stock cannot be negative.");
        Stock = newStock;
    }

    private void UpdateStockFromLots()
    {
        // Si hay lotes registrados, el stock se calcula desde los lotes
        if (_lots.Any())
        {
            Stock = _lots.Sum(l => l.CurrentQuantity);
        }
    }
}
