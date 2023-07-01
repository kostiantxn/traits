namespace Traits.Common.Comparison;

/// <summary>
///     Defines an equivalence relation <c>(• == •)</c> on a type.
/// </summary>
/// <remarks>
///     An equivalence relation is a binary relation, such that, for all
///     <c>a</c>, <c>b</c> and <c>c</c>, the following properties must hold:
///     <list type="number">
///         <item><c>a == a</c> (<em>reflexive</em>).</item>
///         <item><c>a == b</c> if and only if <c>b == a</c> (<em>symmetric</em>).</item>
///         <item><c>a == b</c> and <c>b == c</c> imply <c>a == c</c> (<em>transitive</em>).</item>
///     </list>
/// </remarks>
[Trait]
public interface IEq<S>
{
    /// <summary>
    ///     Checks if <c>x == y</c>.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns><c>true</c> if the two values are equal; otherwise, <c>false</c>.</returns>
    bool Eq(S x, S y);

    /// <summary>
    ///     Checks if <c>x != y</c>.
    /// </summary>
    /// <remarks>
    ///     <c>x != y</c> is defined as <c>!(x == y)</c>.
    /// </remarks>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns><c>true</c> if the two values are not equal; otherwise, <c>false</c>.</returns>
    bool Ne(S x, S y) => !Eq(x, y);
}