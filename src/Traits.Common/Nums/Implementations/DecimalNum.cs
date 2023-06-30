namespace Traits.Common.Nums.Implementations;

/// <summary>
///     Implementation of the <see cref="INum{S}"/> trait for <see cref="decimal"/>.
/// </summary>
public sealed class DecimalNum : INum<decimal>
{
    /// <inheritdoc/>
    public decimal Add(decimal x, decimal y) =>
        x + y;

    /// <inheritdoc/>
    public decimal Sub(decimal x, decimal y) =>
        x - y;

    /// <inheritdoc/>
    public decimal Mul(decimal x, decimal y) =>
        x * y;

    /// <inheritdoc/>
    public decimal Neg(decimal x) =>
        -x;
}