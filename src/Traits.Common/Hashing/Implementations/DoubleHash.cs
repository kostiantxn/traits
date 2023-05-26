namespace Traits.Common.Hashing.Implementations;

public sealed class DoubleHash : IHash<double>
{
    public int Of(double self) =>
        self.GetHashCode();
}