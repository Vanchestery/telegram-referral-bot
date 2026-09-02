using Microsoft.AspNetCore.Mvc;

using ReferralBot.Attributes;
using ReferralBot.Core.Interfaces;

namespace ReferralBot.Controllers;

[ApiController]
[Route("api/partners")]
[PartnersKey]
public class PartnersController(
    IPartnerService partnerService,
    ILogger<PartnersController> logger) : ControllerBase
{
    /// <summary>List of all partners.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        logger.LogDebug("Getting all partners");

        var partners = await partnerService.GetAllPartnersAsync(ct);
        return Ok(partners);
    }

    /// <summary>Partner profile by Telegram ID.</summary>
    [HttpGet("{telegramUserId:long}")]
    public async Task<IActionResult> GetByTelegramId(long telegramUserId, CancellationToken ct)
    {
        logger.LogDebug("Getting partner profile for TelegramUserId: {Id}", telegramUserId);

        var profile = await partnerService.GetProfileAsync(telegramUserId, ct);

        return profile is null
            ? NotFound(new { message = "Partner not found" })
            : Ok(profile);
    }
}
