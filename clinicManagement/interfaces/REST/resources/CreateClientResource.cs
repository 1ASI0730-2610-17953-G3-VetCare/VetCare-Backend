namespace VetCare.clinicManagement.interfaces.REST.resources;

public record CreateClientResource(string FullName, string DocumentId, string Phone, string Email, string Address);
