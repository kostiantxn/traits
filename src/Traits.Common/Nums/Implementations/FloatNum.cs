namespace Traits.Common.Nums.Implementations;

public sealed class FloatNum : INum<float>
{
    public float Add(float x, float y) =>
        x + y;

    public float Sub(float x, float y) =>
        x - y;

    public float Mul(float x, float y) =>
        x * y;

    public float Neg(float x) =>
        -x;
}