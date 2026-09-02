using ReferralBot.Models;
using ReferralBot.Pages.PageResults;

using Telegram.Bot.Types;

namespace ReferralBot.Pages;

/// <summary>
/// Bot page interface.
/// Every screen in the bot implements this interface.
///
/// ViewAsync  — render the page (first show or refresh).
/// HandleAsync — handle the user's action on this page (button press).
/// </summary>
public interface IPage
{
    Task<PageResultBase> ViewAsync(Update update, TelegramUserContext context);
    Task<PageResultBase> HandleAsync(Update update, TelegramUserContext context);
}
