using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.communication.application;
using VetCare.communication.domain.repositories;

namespace VetCare.communication.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly DirectMessageCommandService _messageService;
    private readonly IDirectMessageRepository _repository;

    public MessagesController(DirectMessageCommandService messageService, IDirectMessageRepository repository)
    {
        _messageService = messageService;
        _repository = repository;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
    {
        try
        {
            var vetId = GetCurrentUserId();
            var message = await _messageService.SendMessageAsync(request.ClientId, vetId, request.Channel, request.Content);
            return Ok(message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("client/{clientId}")]
    public async Task<IActionResult> GetClientMessages(int clientId)
    {
        var messages = await _repository.GetByClientIdAsync(clientId);
        return Ok(messages);
    }
}

public record SendMessageRequest(int ClientId, string Channel, string Content);
