namespace Traits.Common.Conversion.Implementations;

public sealed class DoubleIntoDecimal : IInto<double, decimal>
{
    public decimal From(double self) =>
        (decimal) self;
}