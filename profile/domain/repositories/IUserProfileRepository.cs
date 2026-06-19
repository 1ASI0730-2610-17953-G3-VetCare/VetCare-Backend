using VetCare.profile.domain.model.aggregates;

namespace VetCare.profile.domain.repositories;

public interface IUserProfileRepository
{
    Task<UserProfile?> FindByUserIdAsync(int userId);
    Task AddAsync(UserProfile profile);
    Task UpdateAsync(UserProfile profile);
}
