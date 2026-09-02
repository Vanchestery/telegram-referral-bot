namespace ReferralBot.Db.Entities;

/// <summary>
/// Partner's referral link.
/// One account = one link (unique index on AccountId).
/// The URL is formed as: https://t.me/{botName}?start={Key}
/// </summary>
public class ReferralLinkEntity
{
    /// <summary>Primary key.</summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Short string key included in the URL.
    /// Length is set via the KEY_LENGTH environment variable (default 8 characters).
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>AccountId of the link owner. Unique index — one account cannot have two links.</summary>
    public Guid AccountId { get; set; }

    /// <summary>Link creation date. Set by the DB via CURRENT_TIMESTAMP.</summary>
    public DateTime CreatedDate { get; set; }
}
