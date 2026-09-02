using ReferralBot.Models;

namespace ReferralBot.Services;

/// <summary>
/// Course catalogue for the bot UI. Encapsulates Stepik calls (list/details/cover)
/// and caching — pages only see CourseIdTitlePair / CourseDetails.
/// </summary>
public interface ICourseService
{
    Task<List<CourseIdTitlePair>> GetCoursesIdTitleAsync(CancellationToken ct = default);
    Task<CourseDetails?> GetCourseInfoAsync(int courseId, CancellationToken ct = default);
    Task<byte[]?> GetCourseLogoAsync(int courseId, CancellationToken ct = default);
}
