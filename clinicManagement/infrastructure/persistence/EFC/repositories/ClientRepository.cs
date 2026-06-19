using VetCare.clinicManagement.infrastructure.persistence.EFC.context;
using Microsoft.EntityFrameworkCore;
using VetCare.clinicManagement.domain.model.aggregates;
using VetCare.clinicManagement.domain.repositories;

namespace VetCare.clinicManagement.infrastructure.persistence.EFC.repositories;

public class ClientRepository : IClientRepository
{
    private readonly ClinicContext _context;

    public ClientRepository(ClinicContext context)
    {
        _context = context;
    }

    public async Task<Client?> FindByIdAsync(int id)
    {
        return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task AddAsync(Client client)
    {
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
    }

    public Task UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        return Task.CompletedTask;
    }
}
