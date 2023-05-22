namespace Traits.Playground.Examples.Nums;

/// <summary>
///     A trait to describe numeric types.
/// </summary>
[Trait]
internal interface INum<S>
{
    /// <summary>
    ///     Adds two numbers.
    /// </summary>
    S Add(S x, S y);

    /// <summary>
    ///     Subtracts two numbers.
    /// </summary>
    S Sub(S x, S y) => Add(x, Neg(y));

    /// <summary>
    ///     Multiplies two numbers.
    /// </summary>
    S Mul(S x, S y);

    /// <summary>
    ///     Negates a number.
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    S Neg(S x);
}