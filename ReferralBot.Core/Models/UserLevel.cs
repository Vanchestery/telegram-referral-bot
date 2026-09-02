namespace ReferralBot.Core.Models;

/// <summary>
/// Partner level in the referral program.
/// Determined dynamically from the number of referrals who made a purchase.
/// </summary>
public enum UserLevel
{
    /// <summary>0–2 referrals. Bonus: 15%.</summary>
    Intern = 0,

    /// <summary>3–5 referrals. Bonus: 20%.</summary>
    Junior = 1,

    /// <summary>6–10 referrals. Bonus: 25%.</summary>
    Middle = 2,

    /// <summary>11–20 referrals. Bonus: 27%.</summary>
    Senior = 3,

    /// <summary>21+ referrals. Bonus: 30%.</summary>
    Ambassador = 4
}
