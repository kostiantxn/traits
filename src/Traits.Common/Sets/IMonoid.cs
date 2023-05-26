namespace Traits.Common.Sets;

/// <summary>
///     Defines a monoid on a type.
/// </summary>
/// <remarks>
///     A <em>monoid</em> is a semigroup with a neutral element <c>Zero</c>,
///     such that <c>Dot(x, Zero) = Dot(Zero, x) = x</c>, where <c>Dot</c> is
///     an associate binary operation defined by a semigroup, must always hold
///     for any element <c>x</c> of type <see cref="S"/>.
/// </remarks>
[Trait]
public interface IMonoid<[Semigroup] S>
{
    /// <summary>
    ///     Returns the neutral element of type <typeparamref name="S"/>.
    /// </summary>
    S Zero();
}