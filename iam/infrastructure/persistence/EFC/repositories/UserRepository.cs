using VetCare.iam.infrastructure.persistence.EFC.context;
using Microsoft.EntityFrameworkCore;
using VetCare.iam.domain.model.aggregates;
using VetCare.iam.domain.model.valueobjects;
using VetCare.iam.domain.repositories;

namespace VetCare.iam.infrastructure.persistence.EFC.repositories;

public class UserRepository : IUserRepository
{
    private readonly IamContext _context;

    public UserRepository(IamContext context)
    {
        _context = context;
    }

    public async Task<User?> FindByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> FindByEmailAsync(Email email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email.Address == email.Address);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}
