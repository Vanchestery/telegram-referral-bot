namespace ReferralBot.Core.Interfaces;

/// <summary>
/// Partner promo codes for discounted course payments.
/// </summary>
public interface IPromoCodeService
{
    /// <summary>
    /// Hex of the partner's personal promo code for a discounted payment link.
    /// null if the account has no promo code for this course (payment without discount).
    /// </summary>
    Task<string?> GetHexForPaymentAsync(int courseId, Guid accountId, CancellationToken ct = default);
}
