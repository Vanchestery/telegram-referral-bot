namespace ReferralBot.Core.Models;

/// <summary>
/// Manual bonus-balance operation from an administrator.
/// Type: "add" — credit, "deduct" — debit.
/// </summary>
public class ManualOperation
{
    public long TelegramUserId { get; set; }
    public int Amount { get; set; }

    /// <summary>"add" or "deduct".</summary>
    public string Type { get; set; } = string.Empty;
}
