namespace Traits.Playground.Examples.Conversion.Implementations;

internal sealed class IntFromString : IFrom<int, string>
{
    public int Into(string input) =>
        int.Parse(input);
}