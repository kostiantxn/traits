namespace Traits;

[AttributeUsage(AttributeTargets.Class)]
public class ConstraintAttributeAttribute : Attribute
{
    public ConstraintAttributeAttribute(Type type) =>
        Type = type;

    public Type Type { get; }
}