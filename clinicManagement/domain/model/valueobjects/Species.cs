using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.valueobjects;

public class Species : ValueObject
{
    public string Name { get; }

    public Species(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Species name cannot be empty.");
            
        Name = name;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}
