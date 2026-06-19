using VetCare.iam.interfaces.REST.resources;

namespace VetCare.iam.domain.services;

public interface IAuthCommandService
{
    Task<AuthResponseResource> LoginAsync(string emailAddress, string password);
    Task RegisterAsync(string name, string lastName, string emailAddress, string password, string role);
}
