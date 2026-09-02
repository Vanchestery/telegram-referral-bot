using ReferralBot.Pages;

namespace ReferralBot.Services;

/// <summary>
/// Converts a page stack to a list of strings (for DB storage) and back.
///
/// Storage format: List&lt;string&gt; of full type names (Type.FullName).
/// Order: first element = stack bottom, last = top.
/// </summary>
public class PageStackConverter(PagesFactory pagesFactory, ILogger<PageStackConverter> logger)
{
    /// <summary>Stack → List&lt;string&gt; (for saving to the DB).</summary>
    public List<string> ToIds(Stack<IPage> pages)
    {
        // Reverse() — стек хранит вершину первой, нам нужен порядок от дна к вершине
        return pages.Reverse()
            .Select(p => p.GetType().FullName
                ?? throw new InvalidOperationException($"Type {p.GetType()} has no FullName"))
            .ToList();
    }

    /// <summary>List&lt;string&gt; → Stack (when loading from the DB).</summary>
    public Stack<IPage> ToStack(List<string> pageIds)
    {
        if (pageIds.Count == 0)
            return new Stack<IPage>();

        try
        {
            // new Stack<IPage>(IEnumerable) кладёт последний элемент на вершину
            var pages = pageIds.Select(id => pagesFactory.GetPage(id));
            return new Stack<IPage>(pages);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to convert page IDs to stack, returning empty stack");
            return new Stack<IPage>();
        }
    }
}
