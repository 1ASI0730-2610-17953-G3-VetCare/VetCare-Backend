using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.clinicManagement.application;
using VetCare.clinicManagement.interfaces.REST.resources;

namespace VetCare.clinicManagement.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/clients")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly ClientService _clientService;

    public ClientsController(ClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _clientService.GetAllClientsAsync();
        return Ok(clients);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientResource request)
    {
        var client = await _clientService.CreateClientAsync(
            request.FullName, request.DocumentId, request.Phone, request.Email, request.Address);
        return Created("", client);
    }
}
