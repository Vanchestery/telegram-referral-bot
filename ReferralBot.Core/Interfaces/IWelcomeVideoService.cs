namespace ReferralBot.Core.Interfaces;

public interface IWelcomeVideoService
{
    /// <summary>
    /// Returns the Telegram file_id of the active welcome video.
    /// null if no video is uploaded or none is active.
    /// </summary>
    Task<string?> GetActiveFileIdAsync(CancellationToken ct = default);
}
