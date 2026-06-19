namespace VetCare.iam.interfaces.REST.resources;

public record AuthResponseResource(string AccessToken, int Id, string Nombre, string Apellidos, string Email, string[] Roles);
