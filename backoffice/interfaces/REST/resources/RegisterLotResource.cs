namespace VetCare.backoffice.interfaces.REST.resources;

public record RegisterLotResource(string LotNumber, int InitialQuantity, DateTime ExpiryDate);
