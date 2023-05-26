namespace Traits.Common.Nums;

/// <summary>
///     Defines a numeric type which implements the <c>+</c>, <c>-</c> (unary and binary),
///     and <c>*</c> operators.
/// </summary>
[Trait]
public interface INum<S>
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
    S Neg(S x);
}