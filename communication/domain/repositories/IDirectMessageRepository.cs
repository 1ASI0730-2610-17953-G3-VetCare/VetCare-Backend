using VetCare.communication.domain.model.aggregates;

namespace VetCare.communication.domain.repositories;

public interface IDirectMessageRepository
{
    Task AddAsync(DirectMessage message);
    Task<IEnumerable<DirectMessage>> GetByClientIdAsync(int clientId);
}
