namespace Traits.Playground.Examples.Conversion.Implementations;

internal sealed class BoolFromString : IFrom<bool, string>
{
    public bool Into(string input) =>
        bool.Parse(input);
}