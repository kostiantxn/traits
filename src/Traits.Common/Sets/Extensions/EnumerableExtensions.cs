namespace Traits.Common.Sets.Extensions;

/// <summary>
///     Extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Applies the function defined by the <see cref="Semigroup"/> trait to values
    ///     in the enumerable, folding them into a single value.
    /// </summary>
    /// <param name="self">The enumerable to fold.</param>
    /// <typeparam name="T">The type of values in the enumerable.</typeparam>
    /// <returns>The folded value.</returns>
    /// <seealso cref="Enumerable.Aggregate{T}"/>
    public static T Fold<[Monoid] T>(this IEnumerable<T> self) =>
        self.Aggregate(Monoid.Zero<T>(), Semigroup.Dot);
}