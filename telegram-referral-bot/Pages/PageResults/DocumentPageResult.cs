using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages.PageResults;

/// <summary>
/// Page result with a document. Sent via SendDocument.
/// </summary>
public class DocumentPageResult(
    InputFile document,
    string caption,
    InlineKeyboardMarkup replyMarkup,
    IPage? nextPage = null)
    : PageResultBase(caption, replyMarkup, nextPage)
{
    public InputFile Document { get; } = document;
}
