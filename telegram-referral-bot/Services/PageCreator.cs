namespace ReferralBot.Services;

/// <summary>
/// Creates page instances through DI with the correct lifetime.
///
/// Why IServiceScopeFactory instead of IServiceProvider directly?
/// PageCreator itself is registered as Scoped, and pages are also Scoped.
/// Creating via the same scope is correct — the page gets the same dependencies.
/// </summary>
public class PageCreator(IServiceScopeFactory serviceScopeFactory)
{
    public T CreatePage<T>() where T : ReferralBot.Pages.IPage
    {
        // Scope создаётся здесь и живёт вместе со страницей.
        // Не используем using — scope должен жить пока живёт страница.
        var scope = serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}
