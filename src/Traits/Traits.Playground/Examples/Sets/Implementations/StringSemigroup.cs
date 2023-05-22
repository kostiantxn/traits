namespace Traits.Playground.Examples.Sets.Implementations;

internal sealed class StringSemigroup : ISemigroup<string>
{
    public string Dot(string x, string y) =>
        x + y;
}