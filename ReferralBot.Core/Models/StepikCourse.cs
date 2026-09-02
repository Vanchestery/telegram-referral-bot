namespace ReferralBot.Core.Models;

public class StepikCourse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    /// <summary>Numeric price as a string ("68900.00"). Parsed to decimal when needed.</summary>
    public string? Price { get; set; }

    /// <summary>Course cover URL on the Stepik CDN.</summary>
    public string? Cover { get; set; }

    /// <summary>Display price, already with currency ("68900 ₽"). null — free / no price.</summary>
    public string? DisplayPrice { get; set; }

    /// <summary>Course order in the teacher's listing — for stable sorting.</summary>
    public int Position { get; set; }

    // Флаги публикации — фильтруем, чтобы в бот не попали черновики/архив.
    public bool IsPublic { get; set; }
    public bool IsActive { get; set; }
    public bool IsArchived { get; set; }
}
