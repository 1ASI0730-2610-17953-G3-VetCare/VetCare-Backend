using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.aggregates;

public class HospitalizationTask : AggregateRoot
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public string Status { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime TaskDate { get; private set; }
    public string TaskTime { get; private set; }
    public bool Completed { get; private set; }

    protected HospitalizationTask()
    {
        Status = "en_espera";
        Title = string.Empty;
        Description = string.Empty;
        TaskTime = "09:00";
    }

    public HospitalizationTask(
        int patientId,
        string status,
        string title,
        string description,
        DateTime taskDate,
        string taskTime)
    {
        PatientId = patientId;
        Status = status;
        Title = title;
        Description = description;
        TaskDate = taskDate.Date;
        TaskTime = taskTime;
        Completed = false;
    }

    public void ToggleComplete()
    {
        Completed = !Completed;
    }
}
