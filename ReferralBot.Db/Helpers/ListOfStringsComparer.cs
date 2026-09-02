using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ReferralBot.Db.Helpers;

/// <summary>
/// Custom ValueComparer for List&lt;string&gt; stored in a jsonb column.
///
/// Why it exists:
/// EF Core compares object references by default (reference equality).
/// For a jsonb column with List&lt;string&gt; that makes EF Core always
/// treat the value as "changed" and emit extra UPDATE statements.
/// This comparer teaches EF Core to compare list contents, not references.
/// </summary>
public class ListOfStringsComparer() : ValueComparer<List<string>>(
    (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
    list => list == null ? 0 : list.Aggregate(0, (acc, s) => HashCode.Combine(acc, s.GetHashCode())),
    list => list == null ? new List<string>() : list.ToList());
