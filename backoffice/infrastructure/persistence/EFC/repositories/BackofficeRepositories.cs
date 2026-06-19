using VetCare.backoffice.infrastructure.persistence.EFC.context;
using Microsoft.EntityFrameworkCore;
using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.domain.repositories;

namespace VetCare.backoffice.infrastructure.persistence.EFC.repositories;

public class ProductRepository : IProductRepository
{
    private readonly BackofficeContext _context;
    public ProductRepository(BackofficeContext context) { _context = context; }

    public async Task<Product?> FindByIdAsync(int id) => await _context.Products.FindAsync(id);
    public async Task<IEnumerable<Product>> GetAllAsync() =>
        await _context.Products.OrderBy(p => p.Id).ToListAsync();
    public async Task AddAsync(Product product) { await _context.Products.AddAsync(product); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(Product product) { _context.Products.Update(product); await _context.SaveChangesAsync(); }
}

public class EntryRepository : IEntryRepository
{
    private readonly BackofficeContext _context;
    public EntryRepository(BackofficeContext context) { _context = context; }

    public async Task<IEnumerable<Entry>> GetAllAsync() => await _context.Entries.ToListAsync();
    public async Task AddAsync(Entry entry) { await _context.Entries.AddAsync(entry); await _context.SaveChangesAsync(); }
}

public class SupplierRepository : ISupplierRepository
{
    private readonly BackofficeContext _context;
    public SupplierRepository(BackofficeContext context) { _context = context; }

    public async Task<IEnumerable<Supplier>> GetAllAsync() => await _context.Suppliers.ToListAsync();
    public async Task AddAsync(Supplier supplier) { await _context.Suppliers.AddAsync(supplier); await _context.SaveChangesAsync(); }
}
