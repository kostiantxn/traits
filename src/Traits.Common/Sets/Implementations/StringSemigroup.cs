namespace Traits.Common.Sets.Implementations;

public sealed class StringSemigroup : ISemigroup<string>
{
    public string Dot(string x, string y) =>
        x + y;
}