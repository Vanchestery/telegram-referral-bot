using ReferralBot.Core.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace ReferralBot.Services;

/// <summary>
/// Ежедневная рассылка статистики партнёрам (09:00 UTC).
/// BackgroundService — Singleton: Scoped-сервисы только через IServiceScopeFactory.
/// </summary>
public class DailyStatsNotificationService(
    IServiceScopeFactory scopeFactory,
    ILogger<DailyStatsNotificationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("DailyStatsNotificationService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayUntilNextRun();
            logger.LogDebug("Next daily stats run in {Minutes} minutes", delay.TotalMinutes);

            await Task.Delay(delay, stoppingToken);

            if (stoppingToken.IsCancellationRequested) break;

            await SendDailyStatsAsync(stoppingToken);
        }

        logger.LogInformation("DailyStatsNotificationService stopped");
    }

    private async Task SendDailyStatsAsync(CancellationToken ct)
    {
        logger.LogInformation("Starting daily stats notification");

        using var scope = scopeFactory.CreateScope();
        var partnerService = scope.ServiceProvider.GetRequiredService<IPartnerService>();
        var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

        try
        {
            var partners = await partnerService.GetAllPartnersAsync(ct);
            var count = 0;

            foreach (var partner in partners)
            {
                try
                {
                    var message =
                        $"📊 *Ежедневная статистика*\n\n" +
                        $"💰 Баланс: {partner.BonusBalance}₽\n" +
                        $"👥 Рефералов: {partner.InvitedCount}\n" +
                        $"✅ Купили курс: {partner.InvitedPurchasesCount}\n" +
                        $"💵 Всего заработано: {partner.TotalBonusEarned}₽";

                    await botClient.SendMessage(
                        chatId: partner.TelegramUserId,
                        text: message,
                        parseMode: ParseMode.Markdown,
                        cancellationToken: ct);

                    count++;
                    await Task.Delay(50, ct);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to send stats to TelegramUserId: {Id}", partner.TelegramUserId);
                }
            }

            logger.LogInformation("Daily stats sent to {Count} partners", count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during daily stats notification");
        }
    }

    private static TimeSpan GetDelayUntilNextRun()
    {
        var now = DateTime.UtcNow;
        var nextRun = now.Date.AddHours(9);

        if (now >= nextRun)
            nextRun = nextRun.AddDays(1);

        return nextRun - now;
    }
}
