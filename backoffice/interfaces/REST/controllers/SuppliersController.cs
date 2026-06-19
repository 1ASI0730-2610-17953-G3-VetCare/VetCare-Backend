using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.backoffice.application;
using VetCare.backoffice.interfaces.REST.resources;

namespace VetCare.backoffice.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/suppliers")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ProcurementService _procurementService;

    public SuppliersController(ProcurementService procurementService)
    {
        _procurementService = procurementService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _procurementService.GetAllSuppliersAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierResource request)
    {
        var supplier = await _procurementService.CreateSupplierAsync(request.Name, request.Contact, request.Phone, request.Email);
        return Created("", supplier);
    }
}
