namespace ReferralBot.Db.Entities;

/// <summary>
/// Bonus-balance operation log. Append-only — an immutable journal.
/// Stores balance before and after each operation for a full audit trail.
/// </summary>
public class BonusTransactionEntity
{
    /// <summary>Unique transaction identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Account whose balance changed.</summary>
    public Guid AccountId { get; set; }

    /// <summary>Amount of the change: positive = credit, negative = debit.</summary>
    public int Amount { get; set; }

    /// <summary>
    /// Transaction ID from the external payment system (Stepik).
    /// Used for idempotency: do not process the same transaction twice.
    /// </summary>
    public int PaymentTransactionId { get; set; }

    /// <summary>Time of the original payment operation.</summary>
    public DateTime PaymentTime { get; set; }

    /// <summary>Operation type — drives credit/debit business logic.</summary>
    public BonusOperationType OperationType { get; set; }

    /// <summary>Date the transaction was recorded in the system.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Account balance before the operation — for audit and history.</summary>
    public int BalanceBefore { get; set; }

    /// <summary>Account balance after the operation.</summary>
    public int BalanceAfter { get; set; }

    /// <summary>Course ID for CoursePurchase operations. 0 if not applicable.</summary>
    public int PurchasedCourseId { get; set; } = 0;
}
