namespace Traits.Playground.Examples.Hashing.Implementations;

internal sealed class ObjectHash : IHash<object>
{
    public int Of(object self) =>
        self.GetHashCode();
}