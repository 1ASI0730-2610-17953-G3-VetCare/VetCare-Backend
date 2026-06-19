using Microsoft.AspNetCore.Mvc;
using VetCare.iam.application;
using VetCare.iam.interfaces.REST.resources;

namespace VetCare.iam.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginResource request)
    {
        try
        {
            var response = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterResource request)
    {
        try
        {
            await _authService.RegisterAsync(
                request.Name, 
                request.LastName, 
                request.Email, 
                request.Password, 
                request.Role);
                
            return Ok(new { message = "User registered successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
