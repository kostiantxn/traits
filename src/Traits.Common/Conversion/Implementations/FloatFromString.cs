namespace Traits.Common.Conversion.Implementations;

public sealed class FloatFromString : IFrom<float, string>
{
    public float Into(string value) =>
        float.Parse(value);
}