namespace Traits.Common.Comparison;

/// <summary>
///     Defines a total order relation <c>(• &lt;= •)</c> on a type.
/// </summary>
/// <remarks>
///     A total order relation is a binary relation, such that for all
///     <c>a</c>, <c>b</c>, and <c>c</c>, the following properties must hold:
///     <list type="number">
///         <item><c>a &lt;= a</c> (<em>reflexive</em>).</item>
///         <item><c>a &lt;= b</c> and <c>b &lt;= c</c> imply <c>a &lt;= c</c> (<em>transitive</em>).</item>
///         <item><c>a &lt;= b</c> and <c>b &lt;= a</c> imply <c>a == b</c> (<em>antisymmetric</em>).</item>
///         <item><c>a &lt;= b</c> or <c>b &lt;= a</c> (<em>total</em>).</item>
///     </list>
/// </remarks>
[Trait]
public interface IOrder<[Equiv] S>
{
    /// <summary>
    ///     Compares <c>x</c> and <c>y</c>.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns>The ordering between the two values.</returns>
    Ordering Cmp(S x, S y);

    /// <summary>
    ///     Checks if <c>x &lt; y</c>.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns><c>true</c> if <c>x &lt; y</c>; otherwise, <c>false</c>.</returns>
    bool Lt(S x, S y) => Cmp(x, y) is Ordering.Less;

    /// <summary>
    ///     Checks if <c>x &lt;= y</c>.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns><c>true</c> if <c>x &lt;= y</c>; otherwise, <c>false</c>.</returns>
    bool Le(S x, S y) => Cmp(x, y) is Ordering.Less or Ordering.Equal;

    /// <summary>
    ///     Checks if <c>x &gt; y</c>.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns><c>true</c> if <c>x &gt; y</c>; otherwise, <c>false</c>.</returns>
    bool Gt(S x, S y) => Cmp(x, y) is Ordering.Greater;

    /// <summary>
    ///     Checks if <c>x &gt;= y</c>.
    /// </summary>
    /// <param name="x">The left operand.</param>
    /// <param name="y">The right operand.</param>
    /// <returns><c>true</c> if <c>x &gt;= y</c>; otherwise, <c>false</c>.</returns>
    bool Ge(S x, S y) => Cmp(x, y) is Ordering.Greater or Ordering.Equal;

    /// <summary>
    ///     Returns the minimum of <c>x</c> and <c>y</c>.
    /// </summary>
    /// <remarks>
    ///     If <c>x == y</c>, then <c>x</c> will be returned.
    /// </remarks>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <returns>The minimum of <c>x</c> and <c>y</c>.</returns>
    S Min(S x, S y) => Le(x, y) ? x : y;

    /// <summary>
    ///     Returns the maximum of <c>x</c> and <c>y</c>.
    /// </summary>
    /// <remarks>
    ///     If <c>x == y</c>, then <c>x</c> will be returned.
    /// </remarks>
    /// <param name="x">The first value.</param>
    /// <param name="y">The second value.</param>
    /// <returns>The maximum of <c>x</c> and <c>y</c>.</returns>
    S Max(S x, S y) => Ge(x, y) ? x : y;

    /// <summary>
    ///     Restricts <c>x</c> to the <c>[min..max]</c> interval.
    /// </summary>
    /// <param name="x">The value to restrict.</param>
    /// <param name="min">The lower bound of the interval.</param>
    /// <param name="max">The upper bound of the interval.</param>
    /// <returns>The restricted value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <c>min > max</c>.</exception>
    S Clamp(S x, S min, S max) =>
        Le(min, max)
            ? Min(Max(x, min), max)
            : throw new ArgumentOutOfRangeException(nameof(min), "`min <= max` must hold");
}