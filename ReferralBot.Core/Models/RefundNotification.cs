namespace ReferralBot.Core.Models;

/// <summary>
/// Refund notification from an external payment system.
/// Accepted via POST /api/bonus/refund.
/// </summary>
public class RefundNotification
{
    public int TransactionId { get; set; }
    public int CourseId { get; set; }
    public long UserId { get; set; }
    public int Amount { get; set; }
    public DateTime RefundTime { get; set; }
}
