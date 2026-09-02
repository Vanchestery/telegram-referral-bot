using ReferralBot.Models;

using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages.PageResults;

/// <summary>
/// Page render result — everything needed to send a Telegram message.
/// Telegram.Bot 22.x has no common ReplyMarkup base class —
/// we use InlineKeyboardMarkup directly (all bot pages use inline buttons).
/// </summary>
public class PageResultBase
{
    public string Text { get; }
    public InlineKeyboardMarkup ReplyMarkup { get; }
    public ParseMode ParseMode { get; set; } = ParseMode.MarkdownV2;

    /// <summary>
    /// Next page for navigation.
    /// Set in HandleAsync when the user presses a button.
    /// null — stay on the current page.
    /// </summary>
    public IPage? NextPage { get; }

    /// <summary>true if the result contains media (photo, document).</summary>
    public bool IsMedia => this is PhotoPageResult or DocumentPageResult;

    /// <summary>User context — set in the page's ViewAsync.</summary>
    public TelegramUserContext TelegramUserContext { get; set; } = null!;

    public PageResultBase(string text, InlineKeyboardMarkup replyMarkup, IPage? nextPage = null)
    {
        Text = text;
        ReplyMarkup = replyMarkup;
        NextPage = nextPage;
    }
}
