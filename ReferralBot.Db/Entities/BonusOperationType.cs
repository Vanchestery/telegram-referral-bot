namespace ReferralBot.Db.Entities;

/// <summary>
/// Bonus-balance operation type.
/// Stored as int — compactness and filter speed matter for transactions.
/// </summary>
public enum BonusOperationType
{
    /// <summary>Bonus credit to the referrer for a referral's purchase.</summary>
    Purchase = 1,

    /// <summary>Bonus debit when a referral's purchase is refunded.</summary>
    Refund = 2,

    /// <summary>Bonus debit when the partner buys a course themselves.</summary>
    CoursePurchase = 3,

    /// <summary>Manual adjustment by an administrator.</summary>
    ManualOperation = 4,

    /// <summary>Debit via an external request.</summary>
    Debit = 5,

    /// <summary>Credit via an external request.</summary>
    Credit = 6
}
