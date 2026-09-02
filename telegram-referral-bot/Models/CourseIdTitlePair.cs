namespace ReferralBot.Models;

/// <summary>
/// Compact course entry for a list: identifier + title.
/// Full details are loaded separately by Id when the card is opened.
/// </summary>
public record CourseIdTitlePair
{
    public int Id { get; init; }
    public required string Title { get; init; }
}
