namespace ReferralBot.Core.Models;

/// <summary>
/// Purchase notification from an external payment system (Stepik).
/// Accepted via POST /api/bonus/payment.
/// </summary>
public class PaymentNotification
{
    public int TransactionId { get; set; }
    public int CourseId { get; set; }

    /// <summary>Stepik User ID of the buyer.</summary>
    public long UserId { get; set; }

    /// <summary>Purchase amount in rubles.</summary>
    public int Amount { get; set; }

    public DateTime PaymentTime { get; set; }
}
