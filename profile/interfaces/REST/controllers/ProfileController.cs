using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetCare.iam.domain.repositories;
using VetCare.profile.application;
using VetCare.profile.interfaces.REST.resources;

namespace VetCare.profile.interfaces.REST.controllers;

[ApiController]
[Route("api/v1/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly UserProfileService _profileService;
    private readonly ProfileProductivityQueryService _productivityQuery;
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _environment;

    public ProfileController(
        UserProfileService profileService,
        ProfileProductivityQueryService productivityQuery,
        IUserRepository userRepository,
        IWebHostEnvironment environment)
    {
        _profileService = profileService;
        _productivityQuery = productivityQuery;
        _userRepository = userRepository;
        _environment = environment;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.GetOrCreateProfileAsync(userId);
        var user = await _userRepository.FindByIdAsync(userId);

        var productivity = await _productivityQuery.GetMonthlyStatsAsync(
            user?.Name,
            user?.LastName);

        return Ok(new ProfileDetailResource(
            profile.Id,
            profile.UserId,
            profile.Theme,
            profile.Language,
            profile.ReceiveNotifications,
            profile.AvatarUrl,
            new ProfileProductivityResource(
                productivity.MonthlyAppointments,
                productivity.PatientsAttended,
                productivity.MonthlyGoalPercent)));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateProfileResource request)
    {
        var userId = GetCurrentUserId();
        var profile = await _profileService.UpdateProfileAsync(userId, request.Theme, request.Language, request.ReceiveNotifications);
        var user = await _userRepository.FindByIdAsync(userId);

        var productivity = await _productivityQuery.GetMonthlyStatsAsync(
            user?.Name,
            user?.LastName);

        return Ok(new ProfileDetailResource(
            profile.Id,
            profile.UserId,
            profile.Theme,
            profile.Language,
            profile.ReceiveNotifications,
            profile.AvatarUrl,
            new ProfileProductivityResource(
                productivity.MonthlyAppointments,
                productivity.PatientsAttended,
                productivity.MonthlyGoalPercent)));
    }

    [HttpPost("avatar")]
    [RequestSizeLimit(MaxAvatarBytes)]
    public async Task<IActionResult> UploadAvatar(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded." });

        if (file.Length > MaxAvatarBytes)
            return BadRequest(new { message = "File size must be under 2MB." });

        try
        {
            var userId = GetCurrentUserId();
            await using var stream = file.OpenReadStream();
            var avatarUrl = await _profileService.UploadAvatarAsync(
                userId,
                stream,
                file.ContentType,
                _environment.WebRootPath);

            return Ok(new { avatarUrl });
        }
        catch (AvatarUploadException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private const long MaxAvatarBytes = 2 * 1024 * 1024;
}
