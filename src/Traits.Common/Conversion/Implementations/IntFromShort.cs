namespace Traits.Common.Conversion.Implementations;

public sealed class IntFromShort : IFrom<int, short>
{
    public int Into(short value) =>
        value;
}