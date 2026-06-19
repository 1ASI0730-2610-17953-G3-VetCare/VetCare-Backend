using Microsoft.EntityFrameworkCore;
using VetCare.communication.domain.model.aggregates;
using VetCare.communication.domain.repositories;
using VetCare.communication.infrastructure.persistence.EFC.context;

namespace VetCare.communication.infrastructure.persistence.EFC.repositories;

public class DirectMessageRepository : IDirectMessageRepository
{
    private readonly CommunicationContext _context;

    public DirectMessageRepository(CommunicationContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DirectMessage message)
    {
        await _context.DirectMessages.AddAsync(message);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<DirectMessage>> GetByClientIdAsync(int clientId)
    {
        return await _context.DirectMessages
            .Where(m => m.ClientId == clientId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}
