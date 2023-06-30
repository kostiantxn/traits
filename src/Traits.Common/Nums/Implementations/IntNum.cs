namespace Traits.Common.Nums.Implementations;

/// <summary>
///     Implementation of the <see cref="INum{S}"/> trait for <see cref="int"/>.
/// </summary>
public sealed class IntNum : INum<int>
{
    /// <inheritdoc/>
    public int Add(int x, int y) =>
        x + y;

    /// <inheritdoc/>
    public int Sub(int x, int y) =>
        x - y;

    /// <inheritdoc/>
    public int Mul(int x, int y) =>
        x * y;

    /// <inheritdoc/>
    public int Neg(int x) =>
        -x;
}