using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

using ReferralBot.Services.Bot;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace ReferralBot.Controllers;

[ApiController]
public class WebhookController(
    IBotService botService,
    ITelegramBotClient botClient,
    ILogger<WebhookController> logger) : ControllerBase
{
    /// <summary>
    /// Receiver of updates from Telegram.
    /// </summary>
    /// <remarks>
    /// Telegram sends JSON in snake_case (first_name, message_id, ...).
    /// The default ASP.NET Core serializer (camelCase) does NOT map those fields onto
    /// Telegram.Bot types — e.g. first_name does not bind to FirstName,
    /// so model validation fails and the request is rejected with 400 before it reaches here.
    ///
    /// Therefore we do NOT use [FromBody]: we read the body and deserialize Update manually
    /// via JsonBotAPI.Options — Telegram.Bot's own serializer with a snake_case
    /// naming policy and polymorphic converters. The global MVC serializer is left
    /// unchanged so the REST API keeps its camelCase contract.
    /// </remarks>
    [HttpPost("/webhook/update")]
    public async Task<IActionResult> Update(CancellationToken ct)
    {
        Update? update;
        try
        {
            update = await JsonSerializer.DeserializeAsync<Update>(
                Request.Body, JsonBotAPI.Options, ct);
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "Failed to deserialize incoming update");
            return Ok();
        }

        if (update is null)
            return Ok();

        try
        {
            await botService.HandleUpdateAsync(update, botClient, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error in webhook");
            await botService.HandleErrorAsync(ex, ct);
        }

        return Ok();
    }
}
