namespace Traits.Common.Conversion.Implementations;

public sealed class IntFromString : IFrom<int, string>
{
    public int Into(string value) =>
        int.Parse(value);
}