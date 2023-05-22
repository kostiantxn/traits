namespace Traits.Playground.Examples.Nums.Implementations;

internal sealed class DoubleNum : INum<double>
{
    public double Add(double x, double y) =>
        x + y;

    public double Mul(double x, double y) =>
        x * y;

    public double Neg(double x) =>
        -x;
}