using ReferralBot.Models;
using ReferralBot.Pages;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace ReferralBot.Services;

public class CommandsProvider(
    PageCreator pageCreator,
    ILogger<CommandsProvider> logger)
{
    public async Task SetCommandsAsync(ITelegramBotClient client, CancellationToken ct = default)
    {
        var commands = new[]
        {
            new BotCommand { Command = "start", Description = "Начать работу с ботом" }
        };

        await client.SetMyCommands(commands, cancellationToken: ct);
        logger.LogInformation("Bot commands registered");
    }

    public async Task TryHandleCommandAsync(
        Update update,
        TelegramUserContext context,
        ITelegramBotClient client,
        CancellationToken ct = default)
    {
        var text = update.Message?.Text?.Trim();
        if (string.IsNullOrEmpty(text)) return;

        if (text.StartsWith("/start"))
            ResetContext(update, context);

        await Task.CompletedTask;
    }

    private void ResetContext(Update update, TelegramUserContext context)
    {
        context.TelegramId = update.Message!.From!.Id;

        if (context.Pages.Count > 1)
            context.ResetPages();

        if (context.Pages.Count == 0)
            context.AddPage(pageCreator.CreatePage<StartPage>());

        logger.LogDebug("Context reset for user {UserId}", context.TelegramId);
    }
}
