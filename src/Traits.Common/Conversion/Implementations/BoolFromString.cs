namespace Traits.Common.Conversion.Implementations;

public sealed class BoolFromString : IFrom<bool, string>
{
    public bool Into(string value) =>
        bool.Parse(value);
}