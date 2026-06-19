using VetCare.backoffice.domain.model.aggregates;

namespace VetCare.backoffice.domain.repositories;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
}

public interface IEntryRepository
{
    Task<IEnumerable<Entry>> GetAllAsync();
    Task AddAsync(Entry entry);
}

public interface ISupplierRepository
{
    Task<IEnumerable<Supplier>> GetAllAsync();
    Task AddAsync(Supplier supplier);
}
