using VetCare.backoffice.domain.model.aggregates;
using VetCare.backoffice.domain.repositories;

namespace VetCare.backoffice.application;

public class ProcurementService
{
    private readonly ISupplierRepository _supplierRepository;

    public ProcurementService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync() => await _supplierRepository.GetAllAsync();

    public async Task<Supplier> CreateSupplierAsync(string name, string contact, string phone, string email)
    {
        var supplier = new Supplier(name, contact, phone, email);
        await _supplierRepository.AddAsync(supplier);
        return supplier;
    }
}
