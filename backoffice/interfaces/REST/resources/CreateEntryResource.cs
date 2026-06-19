namespace VetCare.backoffice.interfaces.REST.resources;

public record CreateEntryResource(string Type, string Category, decimal Amount, DateTime Date, string Description);
