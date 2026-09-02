using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages.PageResults;

/// <summary>
/// Page result with a photo. Sent via SendPhoto instead of SendMessage.
/// </summary>
public class PhotoPageResult(
    InputFile photo,
    string caption,
    InlineKeyboardMarkup replyMarkup,
    IPage? nextPage = null)
    : PageResultBase(caption, replyMarkup, nextPage)
{
    public InputFile Photo { get; } = photo;
}
