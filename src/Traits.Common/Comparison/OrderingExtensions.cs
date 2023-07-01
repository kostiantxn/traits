namespace Traits.Common.Comparison;

/// <summary>
///     Extensions for <see cref="Ordering"/>.
/// </summary>
public static class OrderingExtensions
{
    /// <summary>
    ///     Reverses the ordering.
    /// </summary>
    /// <param name="self">The ordering to reverse.</param>
    /// <returns>The reversed ordering.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the ordering is unrecognised.</exception>
    public static Ordering Reverse(this Ordering self) =>
        self switch
        {
            Ordering.Less => Ordering.Greater,
            Ordering.Equal => Ordering.Equal,
            Ordering.Greater => Ordering.Less,
            _ => throw new ArgumentOutOfRangeException(nameof(self), self, "Unrecognised ordering"),
        };

    /// <summary>
    ///     Chains two orderings.
    /// </summary>
    /// <param name="self">The first ordering.</param>
    /// <param name="other">The second ordering.</param>
    /// <returns>
    ///     <c>self</c> if it is not <see cref="Ordering.Equal"/>;
    ///     otherwise, <c>other</c>.
    /// </returns>
    public static Ordering Then(this Ordering self, Ordering other) =>
        self is not Ordering.Equal ? self : other;
}