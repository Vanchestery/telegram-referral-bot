namespace ReferralBot.Core.Models;

/// <summary>
/// User status at the domain-model level.
/// Mirrors UserDbStatus from the data layer — the split is intentional:
/// Core must not depend on storage details.
/// </summary>
public enum UserStatus
{
    Active,
    Banned,
    Deleted
}
