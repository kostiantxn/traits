namespace Traits;

[AttributeUsage(AttributeTargets.Class)]
public class ForAttribute : Attribute
{
    public ForAttribute(Type type) =>
        Type = type;

    public Type Type { get; }
}