using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ReferralBot.Attributes;

/// <summary>
/// Authorization filter for external integrations (payment system, admin panel).
/// Checks the secret in the X-Partners-Key header.
///
/// If PARTNERS_API_KEY is not set — skip (local development).
/// </summary>
public class PartnersKeyAttribute : Attribute, IAsyncActionFilter
{
    private const string HeaderName = "X-Partners-Key";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var config = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedKey = config["PARTNERS_API_KEY"];

        if (string.IsNullOrEmpty(expectedKey))
        {
            await next();
            return;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedKey)
            || providedKey != expectedKey)
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Invalid or missing API key" });
            return;
        }

        await next();
    }
}
