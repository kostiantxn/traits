namespace Traits.Playground.Examples.Conversion.Implementations;

internal sealed class IntFromShort : IFrom<int, short>
{
    public int Into(short input) =>
        input;
}