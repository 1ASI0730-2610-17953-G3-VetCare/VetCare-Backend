namespace VetCare.clinicManagement.interfaces.REST.resources;

public class CreateConsultationResource
{
    public int PatientId { get; set; }
    public string? DoctorName { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Analysis { get; set; }
    public string? Plan { get; set; }
    public decimal? Temperature { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Weight { get; set; }
    public int? BodyCondition { get; set; }
    public DateTime Date { get; set; }
    public List<ConsultationItemResource>? Items { get; set; }
}

public class ConsultationItemResource
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
