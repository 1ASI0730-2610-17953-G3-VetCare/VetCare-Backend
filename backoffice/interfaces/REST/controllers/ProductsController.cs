using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.backoffice.application;
using VetCare.backoffice.interfaces.REST.resources;

namespace VetCare.backoffice.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly InventoryService _inventoryService;

    public ProductsController(InventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _inventoryService.GetAllProductsAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductResource request)
    {
        var product = await _inventoryService.CreateProductAsync(request.Code, request.Name, request.Category, request.Stock, request.MinStock, request.Price);
        return Created("", product);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        return Ok(await _inventoryService.GetLowStockProductsAsync());
    }

    [HttpPost("{id}/lots")]
    public async Task<IActionResult> RegisterLot(int id, [FromBody] RegisterLotResource request)
    {
        try
        {
            await _inventoryService.RegisterLotAsync(id, request.LotNumber, request.InitialQuantity, request.ExpiryDate);
            return Ok(new { Message = "Lot registered successfully" });
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/deduct")]
    public async Task<IActionResult> DeductStock(int id, [FromBody] DeductStockResource request)
    {
        try
        {
            await _inventoryService.DeductStockAsync(id, request.Quantity);
            return Ok(new { Message = "Stock deducted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockResource request)
    {
        try
        {
            await _inventoryService.UpdateStockAsync(id, request.Stock);
            return Ok(new { Message = "Stock updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/kardex")]
    public async Task<IActionResult> GetKardex(int id, [FromServices] VetCare.backoffice.infrastructure.persistence.EFC.context.BackofficeContext context)
    {
        var transactions = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
            System.Linq.Queryable.OrderByDescending(
                System.Linq.Queryable.Where(context.InventoryTransactions, t => t.ProductId == id), 
                t => t.Date));

        return Ok(transactions);
    }
}
