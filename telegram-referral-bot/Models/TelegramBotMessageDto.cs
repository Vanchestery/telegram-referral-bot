namespace ReferralBot.Models;

/// <summary>
/// Data of the bot's last message.
/// Used to delete the previous message before sending a new one (clean UI).
/// </summary>
public record TelegramBotMessageDto(int TelegramMessageId, bool IsMedia);
