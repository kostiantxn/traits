namespace Traits;

/// <summary>
///     Used to mark generated classes to indicate which trait they were generated for.
/// </summary>
/// <remarks>
///     Used by the <c>TraitGenerator</c> to mark generated classes.
/// </remarks>
/// <example>
///     <code>
///         [For(typeof(IHash&lt;&gt;))]
///         public class HashAttribute : ConstraintAttribute
///         {
///         }
///     </code>
/// </example>
/// <seealso cref="TraitAttribute"/>
[AttributeUsage(AttributeTargets.Class)]
public class ForAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of <see cref="ForAttribute"/>.
    /// </summary>
    /// <param name="type">The trait the marked class was generated for.</param>
    public ForAttribute(Type type) =>
        Type = type;

    /// <summary>
    ///     The trait the marked class was generated for.
    /// </summary>
    public Type Type { get; }
}