namespace Traits.Playground.Examples.Hashing.Implementations;

internal sealed class Double : IHash<double>
{
    public int Of(double self) =>
        self.GetHashCode();
}