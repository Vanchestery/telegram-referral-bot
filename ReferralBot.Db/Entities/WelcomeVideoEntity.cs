namespace ReferralBot.Db.Entities;

/// <summary>
/// Welcome video sent to new users.
/// Stores the Telegram file_id — after the first upload the video can be resent
/// without uploading the file to Telegram's servers again.
/// </summary>
public class WelcomeVideoEntity
{
    public int Id { get; set; }

    /// <summary>
    /// Telegram file_id — obtained after the first file send to the bot.
    /// Allows resending the video via SendVideo(new InputFileId(fileId)).
    /// </summary>
    public string FileId { get; set; } = string.Empty;

    /// <summary>Local file path on the server for the initial upload.</summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>Date the record was added.</summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Active video version. Only one record should have IsActive=true.
    /// Others are deactivated when a new video is added (soft replace).
    /// </summary>
    public bool IsActive { get; set; } = true;
}
