using VetCare.iam.domain.model.aggregates;
using VetCare.iam.domain.model.valueobjects;

namespace VetCare.iam.domain.repositories;

public interface IUserRepository
{
    Task<User?> FindByIdAsync(int id);
    Task<User?> FindByEmailAsync(Email email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
}
