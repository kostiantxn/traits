namespace Traits.Common.Nums.Implementations;

public sealed class IntNum : INum<int>
{
    public int Add(int x, int y) =>
        x + y;

    public int Sub(int x, int y) =>
        x - y;

    public int Mul(int x, int y) =>
        x * y;

    public int Neg(int x) =>
        -x;
}