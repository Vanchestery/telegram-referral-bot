using System.ComponentModel.DataAnnotations;

namespace ReferralBot.Db.Entities;

/// <summary>
/// Promo code bound to a partner account and a specific course.
/// One account may have at most one promo code per course.
/// </summary>
public class PromoCodeEntity
{
    [Key]
    public int Id { get; set; }

    /// <summary>Account that owns the promo code.</summary>
    public Guid AccountId { get; set; }

    /// <summary>Course ID on the Stepik platform.</summary>
    public int CourseId { get; set; }

    /// <summary>Public name of the promo code.</summary>
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Whether the promo code is active and available for use.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Description of the usage terms.</summary>
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Discount amount (number or text, e.g. "10" or "10%").</summary>
    [MaxLength(50)]
    public string Discount { get; set; } = string.Empty;

    /// <summary>Promo code was created and synced on the Stepik side.</summary>
    public bool IsStepikSide { get; set; } = false;

    /// <summary>true — percentage discount; false — fixed amount.</summary>
    public bool IsPercentDiscount { get; set; } = false;

    /// <summary>
    /// Unique hash for the payment link:
    /// https://stepik.org/a/{courseId}/pay?promo={Hex}
    /// Generated from CourseId + AccountId.
    /// </summary>
    [MaxLength(64)]
    public string Hex { get; set; } = string.Empty;

    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }

    /// <summary>Start of validity. null — valid from creation.</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>End of validity. null — no expiry.</summary>
    public DateTime? ExpireDate { get; set; }

    /// <summary>Stepik User ID for a personal promo code. null — available to everyone.</summary>
    public int? UserId { get; set; }

    /// <summary>Maximum uses per user. null — unlimited.</summary>
    public int? LimitPerUser { get; set; }
}
