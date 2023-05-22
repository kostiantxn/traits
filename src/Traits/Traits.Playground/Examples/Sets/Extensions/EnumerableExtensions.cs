namespace Traits.Playground.Examples.Sets.Extensions;

internal static class EnumerableExtensions
{
    public static T Fold<[Monoid] T>(this IEnumerable<T> self) =>
        self.Aggregate(Monoid.Zero<T>(), Semigroup.Dot);
}