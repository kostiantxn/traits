namespace Traits.Playground.Examples.Hashing.Implementations;

internal sealed class DoubleHash : IHash<double>
{
    public int Of(double self) =>
        self.GetHashCode();
}