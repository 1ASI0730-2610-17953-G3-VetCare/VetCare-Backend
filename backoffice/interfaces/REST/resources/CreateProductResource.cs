namespace VetCare.backoffice.interfaces.REST.resources;

public record CreateProductResource(string Code, string Name, string Category, int Stock, int MinStock, decimal Price);
