namespace ReferralBot.Pages;

/// <summary>
/// Page factory by full type name.
/// Used in PageStackConverter when deserializing the stack from the DB.
///
/// Resolves the type via Type.GetType(fullName) and creates it through DI.
/// </summary>
public class PagesFactory(IServiceProvider services, ILogger<PagesFactory> logger)
{
    public IPage GetPage(string fullTypeName)
    {
        logger.LogDebug("Resolving page type: {TypeName}", fullTypeName);

        var type = Type.GetType(fullTypeName)
            ?? throw new InvalidOperationException($"Page type not found: {fullTypeName}");

        return (IPage)services.GetRequiredService(type);
    }
}
