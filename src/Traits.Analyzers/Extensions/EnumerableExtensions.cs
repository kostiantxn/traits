using System.Collections.Immutable;

namespace Traits.Analyzers.Extensions;

/// <summary>
///     Extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
internal static class EnumerableExtensions
{
    public static int IndexOf<T>(this ImmutableArray<T> self, Func<T, bool> predicate)
    {
        for (var i = 0; i < self.Length; ++i)
            if (predicate(self[i]))
                return i;

        return -1;
    }
}