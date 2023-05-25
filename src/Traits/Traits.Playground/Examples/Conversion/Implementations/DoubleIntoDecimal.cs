namespace Traits.Playground.Examples.Conversion.Implementations;

internal sealed class DoubleIntoDecimal : IInto<double, decimal>
{
    public decimal From(double self) =>
        (decimal) self;
}