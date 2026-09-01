using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ReferralBot.Attributes;

/// <summary>
/// Фильтр авторизации для внешних интеграций (платёжная система, админ-панель).
/// Проверяет секрет в заголовке X-Partners-Key.
///
/// Если PARTNERS_API_KEY не задан — пропускаем (локальная разработка).
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
