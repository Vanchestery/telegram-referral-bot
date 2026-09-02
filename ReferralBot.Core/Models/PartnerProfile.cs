namespace ReferralBot.Core.Models;

/// <summary>
/// Partner profile for display in the bot.
/// Aggregates data from Account — computes level and bonus percentage.
/// </summary>
public class PartnerProfile
{
    public Guid AccountId { get; set; }
    public long TelegramUserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Username { get; set; }
    public int BonusBalance { get; set; }
    public int TotalBonusEarned { get; set; }
    public int InvitedCount { get; set; }
    public int InvitedPurchasesCount { get; set; }
    public UserLevel Level { get; set; }

    /// <summary>Bonus percentage credited to the partner from each referral purchase.</summary>
    public int BonusRate => Level switch
    {
        UserLevel.Intern => 15,
        UserLevel.Junior => 20,
        UserLevel.Middle => 25,
        UserLevel.Senior => 27,
        UserLevel.Ambassador => 30,
        _ => 15
    };
}
