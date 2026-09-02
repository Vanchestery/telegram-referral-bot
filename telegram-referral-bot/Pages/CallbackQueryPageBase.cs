using System.Text.RegularExpressions;

using ReferralBot.Models;
using ReferralBot.Pages.PageResults;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages;

/// <summary>
/// Base class for all bot pages with inline buttons.
///
/// Template method:
///   1. GetRawContentAsync  — page text (overridden in the subclass)
///   2. GetKeyboardAsync     — button matrix (overridden in the subclass)
///   3. GetMediaContentAsync — optional photo (overridden when needed)
///
/// ViewAsync and HandleAsync are implemented here — subclasses need not touch them.
/// </summary>
public abstract class CallbackQueryPageBase : IPage
{
    /// <summary>Page text before MarkdownV2 escaping.</summary>
    protected abstract Task<string> GetRawContentAsync(TelegramUserContext context);

    /// <summary>
    /// Button matrix. Outer array — rows, inner — buttons in the row.
    /// ButtonLinqPage binds a button to a target page.
    /// </summary>
    public abstract Task<ButtonLinqPage[][]> GetKeyboardAsync(TelegramUserContext context);

    /// <summary>Optional media for the page. Default is null (text page).</summary>
    protected virtual Task<InputFile?> GetMediaContentAsync(TelegramUserContext context)
        => Task.FromResult<InputFile?>(null);

    public virtual async Task<PageResultBase> ViewAsync(Update update, TelegramUserContext context)
    {
        var text = await GetRawContentAsync(context);
        var media = await GetMediaContentAsync(context);
        var replyMarkup = await BuildKeyboardAsync(context);
        var escapedText = EscapeMarkdownV2(text);

        if (media is not null)
        {
            return new PhotoPageResult(media, escapedText, replyMarkup)
            {
                TelegramUserContext = context
            };
        }

        return new PageResultBase(escapedText, replyMarkup)
        {
            TelegramUserContext = context
        };
    }

    public virtual async Task<PageResultBase> HandleAsync(Update update, TelegramUserContext context)
    {
        if (update.CallbackQuery?.Data is not string callbackData)
            return await ViewAsync(update, context);

        var keyboard = await GetKeyboardAsync(context);
        var button = keyboard
            .SelectMany(row => row)
            .FirstOrDefault(b => b.Button.CallbackData == callbackData);

        // button is null — кнопка не найдена; button.Page is null — это URL-кнопка
        if (button?.Page is null)
            return await ViewAsync(update, context);

        var nextPageResult = await button.Page.ViewAsync(update, context);
        return PreserveMedia(nextPageResult, nextPageResult.NextPage ?? button.Page);
    }

    /// <summary>
    /// The base HandleAsync used to wrap every result in PageResultBase —
    /// course covers (PhotoPageResult) and documents were lost. We pass
    /// media through as-is and set NextPage for the stack.
    /// </summary>
    private static PageResultBase PreserveMedia(PageResultBase result, IPage nextPage)
    {
        PageResultBase preserved = result switch
        {
            PhotoPageResult photo => new PhotoPageResult(
                photo.Photo, photo.Text, photo.ReplyMarkup, nextPage),
            DocumentPageResult document => new DocumentPageResult(
                document.Document, document.Text, document.ReplyMarkup, nextPage),
            _ => new PageResultBase(result.Text, result.ReplyMarkup, nextPage)
        };

        preserved.ParseMode = result.ParseMode;
        preserved.TelegramUserContext = result.TelegramUserContext;
        return preserved;
    }

    private async Task<InlineKeyboardMarkup> BuildKeyboardAsync(TelegramUserContext context)
    {
        var rows = await GetKeyboardAsync(context);
        return new InlineKeyboardMarkup(
            rows.Select(row => row.Select(b => b.Button).ToArray()).ToArray()
        );
    }

    /// <summary>
    /// Escapes MarkdownV2 special characters.
    /// Telegram requires escaping: _ * [ ] ( ) ~ ` > # + - = | { } . !
    /// </summary>
    private static string EscapeMarkdownV2(string text)
    {
        return Regex.Replace(text, @"([_*\[\]()~`>#+\-=|{}.!\\])", @"\$1");
    }
}
