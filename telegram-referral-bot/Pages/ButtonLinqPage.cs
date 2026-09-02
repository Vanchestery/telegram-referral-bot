using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages;

/// <summary>
/// Binds an InlineKeyboardButton to a target page.
/// Used in GetKeyboardAsync for a declarative button description.
/// </summary>
public class ButtonLinqPage
{
    public InlineKeyboardButton Button { get; }

    /// <summary>
    /// Target page for navigation on press (callback button).
    /// null for a URL button: Telegram opens the link itself; no callback reaches the bot.
    /// </summary>
    public IPage? Page { get; }

    public ButtonLinqPage(InlineKeyboardButton button, IPage page)
    {
        Button = button;
        Page = page;
    }

    /// <summary>Constructor for a URL button (external link, no internal navigation).</summary>
    public ButtonLinqPage(InlineKeyboardButton button)
    {
        Button = button;
        Page = null;
    }
}
