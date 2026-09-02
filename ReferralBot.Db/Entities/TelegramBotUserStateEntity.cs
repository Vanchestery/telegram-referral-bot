namespace ReferralBot.Db.Entities;

/// <summary>
/// User–bot dialogue state.
/// Stores the navigation stack (list of page type names) in a jsonb column.
/// PK = TelegramUserId; one user — one state row.
/// </summary>
public class TelegramBotUserStateEntity
{
    /// <summary>Telegram User ID. Primary key and unique index.</summary>
    public long TelegramUserId { get; set; }

    /// <summary>
    /// Page stack as a list of full type names (Type.FullName).
    /// Stored as jsonb. Order: first element = stack bottom, last = top.
    /// Example: ["ReferralBot.Pages.StartPage", "ReferralBot.Pages.Partner.PartnerHomePage"]
    /// </summary>
    public List<string> PageNames { get; set; } = [];

    /// <summary>Telegram Message ID of the bot's last message — needed to delete it before sending a new one.</summary>
    public int CurrentMessageId { get; set; } = 0;

    /// <summary>Whether the welcome video has already been sent to this user.</summary>
    public bool IsWelcomeMessageSent { get; set; } = false;

    /// <summary>The bot's last message contained media (photo/video) — affects how the next message is sent.</summary>
    public bool IsMediaContent { get; set; } = false;

    /// <summary>Course selected by the user — needed by the course card between updates.</summary>
    public int SelectedCourseId { get; set; } = 0;
}
