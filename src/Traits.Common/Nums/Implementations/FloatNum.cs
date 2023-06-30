namespace Traits.Common.Nums.Implementations;

/// <summary>
///     Implementation of the <see cref="INum{S}"/> trait for <see cref="float"/>.
/// </summary>
public sealed class FloatNum : INum<float>
{
    /// <inheritdoc/>
    public float Add(float x, float y) =>
        x + y;

    /// <inheritdoc/>
    public float Sub(float x, float y) =>
        x - y;

    /// <inheritdoc/>
    public float Mul(float x, float y) =>
        x * y;

    /// <inheritdoc/>
    public float Neg(float x) =>
        -x;
}