namespace Traits.Common.Sets;

/// <summary>
///     Defines a semigroup on a type.
/// </summary>
/// <remarks>
///     A <em>semigroup</em> is a set with an associative binary operation.
/// </remarks>
[Trait]
public interface ISemigroup<S>
{
    /// <summary>
    ///     An associative binary operation defined on type <typeparamref name="S"/>.
    /// </summary>
    S Dot(S x, S y);
}