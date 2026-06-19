using System;
using System.Collections.Generic;
using VetCare.clinicManagement.domain.model.entities;
using VetCare.shared.domain;

namespace VetCare.clinicManagement.domain.model.aggregates;

public class Consultation : AggregateRoot
{
    public int Id { get; private set; }
    public int PatientId { get; private set; }
    public string DoctorName { get; private set; }
    public string Type { get; private set; } // e.g. Consulta General, Vacunación, Emergencia, Chequeo Preventivo
    public string Status { get; private set; } // e.g. pendiente, en_proceso, completada, urgente, critico

    // SOAP Format
    public string? Subjective { get; private set; } // Síntomas
    public string? Objective { get; private set; }  // Signos
    public string? Analysis { get; private set; }   // Diagnóstico
    public string? Plan { get; private set; }       // Tratamiento y recetas

    // Vital Signs
    public decimal? Temperature { get; private set; }
    public int? HeartRate { get; private set; }
    public decimal? Weight { get; private set; }
    public int? BodyCondition { get; private set; } // 1 to 5 scale

    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<ConsultationItem> _items = new();
    public IReadOnlyCollection<ConsultationItem> Items => _items.AsReadOnly();

    protected Consultation() { } // EF Core

    public Consultation(
        int patientId,
        string doctorName,
        string type,
        string status,
        string? subjective,
        string? objective,
        string? analysis,
        string? plan,
        decimal? temperature,
        int? heartRate,
        decimal? weight,
        int? bodyCondition,
        DateTime date)
    {
        PatientId = patientId;
        DoctorName = doctorName;
        Type = type;
        Status = status;
        Subjective = subjective;
        Objective = objective;
        Analysis = analysis;
        Plan = plan;
        Temperature = temperature;
        HeartRate = heartRate;
        Weight = weight;
        BodyCondition = bodyCondition;
        Date = date;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string doctorName,
        string type,
        string status,
        string? subjective,
        string? objective,
        string? analysis,
        string? plan,
        decimal? temperature,
        int? heartRate,
        decimal? weight,
        int? bodyCondition,
        DateTime date)
    {
        DoctorName = doctorName;
        Type = type;
        Status = status;
        Subjective = subjective;
        Objective = objective;
        Analysis = analysis;
        Plan = plan;
        Temperature = temperature;
        HeartRate = heartRate;
        Weight = weight;
        BodyCondition = bodyCondition;
        Date = date;
    }

    public void AddItem(int productId, string productName, int quantity, decimal unitPrice)
    {
        _items.Add(new ConsultationItem(productId, productName, quantity, unitPrice));
    }

    public void MarkRegistered()
    {
        AddDomainEvent(new VetCare.clinicManagement.Domain.Model.Events.ConsultationRegisteredEvent(this));
    }

    public void Complete()
    {
        if (Status == "completada") return;
        Status = "completada";
        
        AddDomainEvent(new VetCare.clinicManagement.Domain.Model.Events.ConsultationClosedEvent(this));
    }
}
