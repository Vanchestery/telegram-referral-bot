using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using ReferralBot.Core.Interfaces;
using ReferralBot.Core.Mappings;
using ReferralBot.Core.Services;
using ReferralBot.Db.Context;
using ReferralBot.Db.Interfaces;
using ReferralBot.Db.Storage;
using ReferralBot.Pages;
using ReferralBot.Services;
using ReferralBot.Services.Bot;

using Telegram.Bot;

namespace ReferralBot.Extensions;

public static class ContainerConfigurator
{
    public static void Configure(IServiceCollection services, IConfiguration configuration)
    {
        ConfigureBot(services);
        ConfigureTelegramBotClient(services);
        ConfigureDbContext(services);
        RegisterStorages(services);
        RegisterCoreServices(services);
        RegisterBotServices(services);
        RegisterPages(services);
        ConfigureAutoMapper(services);

        services.AddSingleton<IHostedService>(sp =>
            new WebHookConfigurator(
                sp.GetRequiredService<IServiceScopeFactory>(),
                sp.GetRequiredService<ILogger<WebHookConfigurator>>()));
    }

    private static void ConfigureBot(IServiceCollection services)
    {
        services.AddOptions<BotConfiguration>()
            .Configure<IConfiguration>((cfg, config) =>
            {
                cfg.Token = config["REF_BOT_KEY"] ?? string.Empty;
                cfg.WebhookUrl = WebhookUrlResolver.Resolve(config) ?? string.Empty;
            })
            .Validate(cfg =>
                !string.IsNullOrEmpty(cfg.Token) &&
                !string.IsNullOrEmpty(cfg.WebhookUrl),
                "REF_BOT_KEY and REF_BOT_WEBHOOK_URL must be set")
            .ValidateOnStart();
    }

    private static void ConfigureTelegramBotClient(IServiceCollection services)
    {
        services.AddHttpClient("telegram_bot")
            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
            {
                var config = sp.GetRequiredService<IOptions<BotConfiguration>>().Value;
                return new TelegramBotClient(config.Token, httpClient);
            });
    }

    private static void ConfigureDbContext(IServiceCollection services)
    {
        services.AddDbContext<DatabaseContext>((sp, options) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var connectionString = config["POSTGRES_REFERRALBOT_DB"]
                ?? throw new InvalidOperationException("POSTGRES_REFERRALBOT_DB is not set");

            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
            });
        },
        contextLifetime: ServiceLifetime.Scoped,
        optionsLifetime: ServiceLifetime.Singleton);
    }

    private static void RegisterStorages(IServiceCollection services)
    {
        services.AddScoped<ITelegramBotUsersStorage, TelegramBotUsersStorage>();
        services.AddScoped<ITelegramUserStatesStorage, TelegramUserStatesStorage>();
        services.AddScoped<IAccountsStorage, AccountsStorage>();
        services.AddScoped<IReferralLinksStorage, ReferralLinksStorage>();
        services.AddScoped<IBonusTransactionStorage, BonusTransactionStorage>();
        services.AddScoped<IPromoCodesStorage, PromoCodesStorage>();
        services.AddScoped<IWelcomeVideoStorage, WelcomeVideoStorage>();
    }

    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddScoped<ITelegramBotUserService, TelegramBotUserService>();
        services.AddScoped<ITelegramBotUserStatesService, TelegramBotUserStatesService>();
    }

    private static void RegisterBotServices(IServiceCollection services)
    {
        services.AddScoped<IBotService, BotService>();
        services.AddScoped<CommandsProvider>();
        services.AddScoped<PageCreator>();
        services.AddScoped<PageStackConverter>();
        services.AddScoped<TelegramUserContextConverter>();
        services.AddScoped<PagesFactory>();
    }

    /// <summary>
    /// Регистрирует все страницы бота через рефлексию.
    /// Любой класс реализующий IPage и не являющийся абстрактным — попадёт в DI.
    /// </summary>
    private static void RegisterPages(IServiceCollection services)
    {
        var pageTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => !t.IsAbstract && typeof(IPage).IsAssignableFrom(t));

        foreach (var type in pageTypes)
            services.AddScoped(type);
    }

    private static void ConfigureAutoMapper(IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<TelegramBotUserProfile>();
            cfg.AddProfile<TelegramBotUserStateProfile>();
        });
    }
}
