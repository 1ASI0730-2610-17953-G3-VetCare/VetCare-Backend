using VetCare.iam.domain.model.aggregates;
using VetCare.iam.domain.model.valueobjects;
using VetCare.iam.domain.repositories;
using VetCare.iam.domain.services;
using VetCare.iam.interfaces.REST.resources;

namespace VetCare.iam.application;

public class AuthService : IAuthCommandService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseResource> LoginAsync(string emailAddress, string password)
    {
        var email = new Email(emailAddress);
        var user = await _userRepository.FindByEmailAsync(email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponseResource(
            token,
            user.Id,
            user.Name,
            user.LastName,
            user.Email.Address,
            new[] { user.Role.ToLowerInvariant() }
        );
    }

    public async Task RegisterAsync(string name, string lastName, string emailAddress, string password, string role)
    {
        var email = new Email(emailAddress);
        var existingUser = await _userRepository.FindByEmailAsync(email);
        
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email already in use.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User(name, lastName, email, passwordHash, role);

        await _userRepository.AddAsync(user);
    }
}
