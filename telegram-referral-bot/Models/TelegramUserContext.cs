using ReferralBot.Pages;

namespace ReferralBot.Models;

/// <summary>
/// User–bot interaction context.
/// Lives in memory for a single request — persisted via TelegramBotUserStatesService.
///
/// The Pages stack is the navigation backbone:
///   - top = current page (CurrentPage)
///   - BackwardDummyPage pops the stack → returns to the previous page
///   - ResetPages() leaves only the stack bottom (the start page)
/// </summary>
public class TelegramUserContext
{
    public long TelegramId { get; set; }

    /// <summary>Page stack. Top = current page.</summary>
    public Stack<IPage> Pages { get; set; } = new();

    /// <summary>The bot's last message — needed to delete it before sending a new one.</summary>
    public TelegramBotMessageDto? LastMessage { get; set; }

    /// <summary>Action history for debugging.</summary>
    public List<string> ActionsHistory { get; set; } = [];

    /// <summary>Course selected by the user (for course pages).</summary>
    public int SelectedCourseId { get; set; }

    /// <summary>Whether the welcome video has already been sent.</summary>
    public bool IsWelcomeMessageSent { get; set; }

    /// <summary>Current page — top of the stack.</summary>
    public IPage CurrentPage => Pages.Peek();

    /// <summary>
    /// Pushes a page onto the stack.
    /// Duplicate guard: does not add if the type matches the current page.
    /// </summary>
    public void AddPage(IPage page)
    {
        if (Pages.Count == 0 || CurrentPage.GetType() != page.GetType())
            Pages.Push(page);
    }

    /// <summary>
    /// Resets the stack to the start page (leaves only the bottom).
    /// Called on /start.
    /// </summary>
    public void ResetPages()
    {
        if (Pages.Count == 0) return;

        while (Pages.Count > 1)
            Pages.Pop();
    }
}
