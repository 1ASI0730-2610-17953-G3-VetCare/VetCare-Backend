using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.valueobjects;

public class Weight : ValueObject
{
    public double Value { get; }
    public string Unit { get; } // e.g., "kg"

    public Weight(double value, string unit = "kg")
    {
        if (value <= 0)
            throw new ArgumentException("Weight must be greater than zero.");
            
        Value = value;
        Unit = unit;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
        yield return Unit;
    }
}
