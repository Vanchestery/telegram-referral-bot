namespace ReferralBot.Extensions;

/// <summary>
/// Telegram bot configuration.
/// Values come from environment variables, user-secrets, and appsettings.
/// </summary>
public class BotConfiguration
{
    public string Token { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
}
