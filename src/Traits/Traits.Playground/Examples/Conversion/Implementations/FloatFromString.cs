namespace Traits.Playground.Examples.Conversion.Implementations;

internal sealed class FloatFromString : IFrom<float, string>
{
    public float Into(string input) =>
        float.Parse(input);
}