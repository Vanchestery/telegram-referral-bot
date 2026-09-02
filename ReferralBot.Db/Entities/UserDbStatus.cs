namespace ReferralBot.Db.Entities;

/// <summary>
/// User account status in the database.
/// Stored as a string (HasConversion) — readable without magic numbers.
/// </summary>
public enum UserDbStatus
{
    Active,
    Banned,
    Deleted
}
