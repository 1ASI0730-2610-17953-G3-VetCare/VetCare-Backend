namespace VetCare.clinicManagement.interfaces.REST.resources;

public record CreatePatientResource(string Name, string Species, int Age, double Weight, int OwnerId);
