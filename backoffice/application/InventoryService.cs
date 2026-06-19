using Microsoft.EntityFrameworkCore;
using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.Domain.Model.Entities;
using VetCare.backoffice.domain.repositories;
using VetCare.backoffice.infrastructure.persistence.EFC.context;
using VetCare.shared.persistence.EFC.extensions.eventDispatcher;
using VetCare.shared.domain;

namespace VetCare.backoffice.application;

public class InventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly BackofficeContext _context;
    private readonly IDomainEventDispatcher _dispatcher;

    public InventoryService(IProductRepository productRepository, BackofficeContext context, IDomainEventDispatcher dispatcher)
    {
        _productRepository = productRepository;
        _context = context;
        _dispatcher = dispatcher;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync() => await _productRepository.GetAllAsync();

    public async Task<Product> CreateProductAsync(string code, string name, string category, int stock, int minStock, decimal price)
    {
        var product = new Product(code, name, category, stock, minStock, price);
        await _productRepository.AddAsync(product);
        await _context.SaveChangesAsync();
        await _dispatcher.DispatchEventsAsync(new[] { product });
        return product;
    }

    public async Task UpdateStockAsync(int id, int newStock)
    {
        var product = await _productRepository.FindByIdAsync(id);
        if (product == null) throw new ArgumentException("Product not found");
        
        product.UpdateStock(newStock);
        await _productRepository.UpdateAsync(product);
    }

    public async Task RegisterLotAsync(int productId, string lotNumber, int initialQuantity, DateTime expiryDate)
    {
        var product = await _productRepository.FindByIdAsync(productId);
        if (product == null) throw new ArgumentException("Product not found");

        var lot = new VetCare.backoffice.Domain.Model.Entities.ProductLot(lotNumber, initialQuantity, expiryDate);
        product.AddLot(lot);
        
        await _productRepository.UpdateAsync(product);
        await _dispatcher.DispatchEventsAsync(new[] { product });
    }

    public async Task DeductStockAsync(int productId, int quantity)
    {
        var product = await _productRepository.FindByIdAsync(productId);
        if (product == null) throw new ArgumentException("Product not found");

        product.DeductStock(quantity);
        await _productRepository.UpdateAsync(product);
        await _dispatcher.DispatchEventsAsync(new[] { product });
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Where(p => p.Stock <= p.MinStock);
    }
}
