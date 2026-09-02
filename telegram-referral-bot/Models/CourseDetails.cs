namespace ReferralBot.Models;

/// <summary>
/// Detailed course card for display in the bot: title, short description, and price.
/// </summary>
public record CourseDetails
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;

    /// <summary>Price in the base currency. 0 — unspecified / free.</summary>
    public decimal Price { get; init; }

    /// <summary>Absolute cover URL on the Stepik CDN. null if there is no cover.</summary>
    public string? CoverUrl { get; init; }
}
