using ReferralBot.Core.Models;

namespace ReferralBot.Services;

/// <summary>
/// Stepik API client contract.
///
/// Split out of StepikApiClient so dependent services (CourseService)
/// can be unit-tested with a mock — without real HTTP calls to Stepik.
/// </summary>
public interface IStepikApiClient
{
    /// <summary>
    /// OAuth2 token via client_credentials.
    /// Returns null if credentials are missing or the request fails.
    /// </summary>
    Task<string?> GetAccessTokenAsync(CancellationToken ct = default);

    /// <summary>
    /// Teacher's courses (all pages). accessToken is optional — public courses
    /// are available without auth. Returns an empty collection on error.
    /// </summary>
    Task<IEnumerable<StepikCourse>> GetTeacherCoursesAsync(
        int teacherId, string? accessToken = null, CancellationToken ct = default);

    /// <summary>Single course by id (details). null on error. Token is optional.</summary>
    Task<StepikCourse?> GetCourseByIdAsync(
        int courseId, string? accessToken = null, CancellationToken ct = default);
}
