using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

using ReferralBot.Core.Models;

namespace ReferralBot.Services;

/// <summary>
/// Typed HTTP client for the Stepik API.
/// Registered via IHttpClientFactory with a Polly retry policy.
///
/// Why IHttpClientFactory instead of new HttpClient():
/// new HttpClient() creates a new socket each time — under high load this
/// causes socket exhaustion. IHttpClientFactory manages the HttpMessageHandler pool.
/// </summary>
public class StepikApiClient(HttpClient httpClient, IConfiguration config, ILogger<StepikApiClient> logger)
    : IStepikApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true
    };

    public async Task<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        var clientId = config["STEPIK_CLIENT_ID"];
        var clientSecret = config["STEPIK_CLIENT_SECRET"];

        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
        {
            logger.LogWarning("Stepik API credentials not configured");
            return null;
        }

        var credentials = Convert.ToBase64String(
            System.Text.Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://stepik.org/oauth2/token/")
        {
            Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            ]),
            Headers = { Authorization = new AuthenticationHeaderValue("Basic", credentials) }
        };

        try
        {
            var response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(json, JsonOptions);

            return tokenResponse?.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to get Stepik access token");
            return null;
        }
    }

    public async Task<IEnumerable<StepikCourse>> GetTeacherCoursesAsync(
        int teacherId, string? accessToken = null, CancellationToken ct = default)
    {
        const int maxPages = 20;
        var all = new List<StepikCourse>();

        for (var page = 1; page <= maxPages; page++)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get, $"https://stepik.org/api/courses?teacher={teacherId}&page={page}");

            if (!string.IsNullOrEmpty(accessToken))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var response = await httpClient.SendAsync(request, ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                var root = JsonSerializer.Deserialize<CoursesResponse>(json, JsonOptions);

                if (root?.Courses is { Count: > 0 })
                    all.AddRange(root.Courses);

                if (root?.Meta?.HasNext != true)
                    break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch courses page {Page} for teacher {TeacherId}", page, teacherId);
                break;
            }
        }

        logger.LogInformation("Fetched {Count} courses for teacher {TeacherId}", all.Count, teacherId);
        return all;
    }

    public async Task<StepikCourse?> GetCourseByIdAsync(
        int courseId, string? accessToken = null, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get, $"https://stepik.org/api/courses/{courseId}");

        if (!string.IsNullOrEmpty(accessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            var response = await httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);
            var root = JsonSerializer.Deserialize<CoursesResponse>(json, JsonOptions);
            return root?.Courses?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to fetch course {CourseId}", courseId);
            return null;
        }
    }

    private record TokenResponse([property: JsonPropertyName("access_token")] string AccessToken);
    private record CoursesMeta([property: JsonPropertyName("has_next")] bool HasNext);
    private record CoursesResponse(
        [property: JsonPropertyName("meta")] CoursesMeta? Meta,
        [property: JsonPropertyName("courses")] List<StepikCourse>? Courses);
}
