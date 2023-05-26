namespace Traits.Common.Nums.Implementations;

public sealed class DecimalNum : INum<decimal>
{
    public decimal Add(decimal x, decimal y) =>
        x + y;

    public decimal Sub(decimal x, decimal y) =>
        x - y;

    public decimal Mul(decimal x, decimal y) =>
        x * y;

    public decimal Neg(decimal x) =>
        -x;
}