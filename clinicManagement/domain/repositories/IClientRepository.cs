using VetCare.clinicManagement.domain.model.aggregates;

namespace VetCare.clinicManagement.domain.repositories;

public interface IClientRepository
{
    Task<Client?> FindByIdAsync(int id);
    Task<IEnumerable<Client>> GetAllAsync();
    Task AddAsync(Client client);
    Task UpdateAsync(Client client);
}
