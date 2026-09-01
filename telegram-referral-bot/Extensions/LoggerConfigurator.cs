using Serilog;
using Serilog.Formatting.Compact;

namespace ReferralBot.Extensions;

/// <summary>
/// Serilog: консоль + дневные файлы Compact JSON (7 дней).
/// </summary>
public static class LoggerConfigurator
{
    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    formatter: new CompactJsonFormatter(),
                    path: Path.Combine(AppContext.BaseDirectory, "logs", "referralbot-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7);
        });
    }
}
