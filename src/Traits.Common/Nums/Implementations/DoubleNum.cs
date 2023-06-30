namespace Traits.Common.Nums.Implementations;

/// <summary>
///     Implementation of the <see cref="INum{S}"/> trait for <see cref="double"/>.
/// </summary>
public sealed class DoubleNum : INum<double>
{
    /// <inheritdoc/>
    public double Add(double x, double y) =>
        x + y;

    /// <inheritdoc/>
    public double Sub(double x, double y) =>
        x - y;

    /// <inheritdoc/>
    public double Mul(double x, double y) =>
        x * y;

    /// <inheritdoc/>
    public double Neg(double x) =>
        -x;
}