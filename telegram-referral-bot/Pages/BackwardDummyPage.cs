using ReferralBot.Models;
using ReferralBot.Pages.PageResults;

using Telegram.Bot.Types;

namespace ReferralBot.Pages;

/// <summary>
/// Service page — implements the "Back" button.
/// On invoke, pops the top page from the stack and renders the new top.
///
/// Not registered in PageNames — used only as a navigation marker.
/// </summary>
public class BackwardDummyPage : IPage
{
    public async Task<PageResultBase> ViewAsync(Update update, TelegramUserContext context)
    {
        if (context.Pages.Count > 1)
            context.Pages.Pop();

        return await context.CurrentPage.ViewAsync(update, context);
    }

    public async Task<PageResultBase> HandleAsync(Update update, TelegramUserContext context)
        => await ViewAsync(update, context);
}
