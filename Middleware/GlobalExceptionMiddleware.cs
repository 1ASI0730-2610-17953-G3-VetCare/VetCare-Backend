using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using VetCare.API.Resources;
using VetCare.shared.domain.exceptions;

namespace VetCare.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IStringLocalizer<SharedResource> _localizer;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IStringLocalizer<SharedResource> localizer)
    {
        _next = next;
        _logger = logger;
        _localizer = localizer;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = _localizer["UnexpectedServerError"];

        switch (exception)
        {
            case DomainException:
                statusCode = HttpStatusCode.BadRequest;
                message = _localizer["ValidationError"];
                break;
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = _localizer["ResourceNotFound"];
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = _localizer["UnauthorizedAccess"];
                break;
        }

        context.Response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            statusCode = (int)statusCode
        });

        return context.Response.WriteAsync(result);
    }
}
