using VetCare.shared.domain;

namespace VetCare.scheduling.domain.model.valueobjects;

public class TimeSlot : ValueObject
{
    public TimeSpan StartTime { get; }
    public TimeSpan EndTime { get; }

    public TimeSlot(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("StartTime must be before EndTime.");
            
        StartTime = startTime;
        EndTime = endTime;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
    }
}
