namespace Traits.Playground.Examples.Hashing.Implementations;

internal sealed class Object : IHash<object>
{
    public int Of(object self) =>
        self.GetHashCode();
}