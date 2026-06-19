using VetCare.shared.domain;
using VetCare.iam.domain.model.valueobjects;

namespace VetCare.iam.domain.model.aggregates;

public class User : AggregateRoot
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Role { get; private set; } // e.g., "admin", "veterinario"

    protected User() { } // For EF Core

    public User(string name, string lastName, Email email, string passwordHash, string role)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void UpdateName(string name, string lastName)
    {
        Name = name;
        LastName = lastName;
    }
}
