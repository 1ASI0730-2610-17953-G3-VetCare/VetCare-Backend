using VetCare.profile.infrastructure.persistence.EFC.context;
using Microsoft.EntityFrameworkCore;
using VetCare.profile.domain.model.aggregates;
using VetCare.profile.domain.repositories;

namespace VetCare.profile.infrastructure.persistence.EFC.repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly ProfileContext _context;
    public UserProfileRepository(ProfileContext context) { _context = context; }

    public async Task<UserProfile?> FindByUserIdAsync(int userId) => await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
    public async Task AddAsync(UserProfile profile) { await _context.UserProfiles.AddAsync(profile); await _context.SaveChangesAsync(); }
    public async Task UpdateAsync(UserProfile profile) { _context.UserProfiles.Update(profile); await _context.SaveChangesAsync(); }
}
