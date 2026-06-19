using VetCare.iam.domain.model.aggregates;

namespace VetCare.iam.application;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
