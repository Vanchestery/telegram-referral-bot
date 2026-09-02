using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ReferralBot.Db.Context;

/// <summary>
/// Factory for creating DatabaseContext when running dotnet ef commands.
/// Needed because DatabaseContext lives in a separate project with no entry point.
///
/// Usage:
///   docker compose up -d db
///   dotnet ef database update --project ReferralBot.Db
/// </summary>
public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("POSTGRES_REFERRALBOT_DB")
            ?? "Host=localhost;Port=5434;Database=referralbot;Username=postgres;Password=postgres";

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new DatabaseContext(optionsBuilder.Options);
    }
}
