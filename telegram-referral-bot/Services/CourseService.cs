using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

using ReferralBot.Core.Models;
using ReferralBot.Models;

namespace ReferralBot.Services;

/// <summary>
/// Course catalogue on top of the Stepik API + IMemoryCache.
///
/// Cache matters because a course card fetches data twice per render
/// (GetCourseInfoAsync for text and GetCourseLogoAsync for the photo) — without cache
/// that is two Stepik calls per view.
/// </summary>
public class CourseService(
    IStepikApiClient stepik,
    IHttpClientFactory httpClientFactory,
    IMemoryCache cache,
    IConfiguration config,
    ILogger<CourseService> logger) : ICourseService
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(15);

    public async Task<List<CourseIdTitlePair>> GetCoursesIdTitleAsync(CancellationToken ct = default)
    {
        var teacherId = config.GetValue<int>("STEPIK_TEACHER_ID");
        if (teacherId <= 0)
        {
            logger.LogWarning("STEPIK_TEACHER_ID не задан — список курсов пуст");
            return [];
        }

        var list = await cache.GetOrCreateAsync($"courses:list:{teacherId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = Ttl;
            var courses = await stepik.GetTeacherCoursesAsync(teacherId, null, ct);
            return courses
                .Where(c => c is { IsPublic: true, IsActive: true, IsArchived: false })
                .OrderBy(c => c.Position)
                .Select(c => new CourseIdTitlePair { Id = c.Id, Title = c.Title })
                .ToList();
        });

        return list ?? [];
    }

    public async Task<CourseDetails?> GetCourseInfoAsync(int courseId, CancellationToken ct = default)
    {
        var course = await GetCourseCachedAsync(courseId, ct);
        if (course is null)
            return null;

        decimal.TryParse(course.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price);

        return new CourseDetails
        {
            Id = course.Id,
            Title = course.Title,
            Summary = course.Summary,
            Price = price,
            CoverUrl = NormalizeCoverUrl(course.Cover)
        };
    }

    public async Task<byte[]?> GetCourseLogoAsync(int courseId, CancellationToken ct = default)
    {
        var course = await GetCourseCachedAsync(courseId, ct);
        if (course is null)
            return null;

        var coverUrl = NormalizeCoverUrl(course.Cover);
        if (coverUrl is null)
            return null;

        try
        {
            var client = httpClientFactory.CreateClient("stepik-covers");
            return await client.GetByteArrayAsync(coverUrl, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Не удалось скачать обложку курса {CourseId}", courseId);
            return null;
        }
    }

    private async Task<StepikCourse?> GetCourseCachedAsync(int courseId, CancellationToken ct)
        => await cache.GetOrCreateAsync($"course:{courseId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = Ttl;
            return await stepik.GetCourseByIdAsync(courseId, null, ct);
        });

    private static string? NormalizeCoverUrl(string? cover)
    {
        if (string.IsNullOrWhiteSpace(cover))
            return null;
        if (cover.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || cover.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return cover;
        if (cover.StartsWith("//"))
            return "https:" + cover;
        if (cover.StartsWith('/'))
            return "https://stepik.org" + cover;
        return "https://stepik.org/" + cover;
    }
}
