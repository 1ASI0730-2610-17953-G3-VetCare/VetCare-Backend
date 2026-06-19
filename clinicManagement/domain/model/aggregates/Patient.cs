using VetCare.shared.domain;
using VetCare.clinicManagement.domain.model.valueobjects;

namespace VetCare.clinicManagement.domain.model.aggregates;

public class Patient : AggregateRoot
{
    public int Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public Species Species { get; private set; }
    public string Status { get; private set; }
    public int Age { get; private set; }
    public Weight Weight { get; private set; }
    public int OwnerId { get; private set; } // Referencing Client by ID

    protected Patient() { } // EF Core

    public Patient(string code, string name, Species species, int age, Weight weight, int ownerId)
    {
        Code = code;
        Name = name;
        Species = species;
        Age = age;
        Weight = weight;
        OwnerId = ownerId;
        Status = "Activo";
    }

    public void UpdateClinicalProfile(int age, double weightValue)
    {
        if (age < 0)
            throw new ArgumentException("Age must be zero or greater.");

        Age = age;
        Weight = new Weight(weightValue);
    }

    public void MarkInTreatment()
    {
        Status = "Tratamiento";
    }

    public void MarkAsActive()
    {
        Status = "Activo";
    }
}
