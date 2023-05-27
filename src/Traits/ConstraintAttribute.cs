namespace Traits;

/// <summary>
///     Inherited by generated trait constraint attributes.
/// </summary>
/// <remarks>
///     Used by the <c>TraitGenerator</c>.
/// </remarks>
/// <seealso cref="TraitAttribute"/>
[AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = true)]
public abstract class ConstraintAttribute : Attribute
{
}