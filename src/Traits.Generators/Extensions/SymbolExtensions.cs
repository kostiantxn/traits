using Microsoft.CodeAnalysis;

namespace Traits.Generators.Extensions;

/// <summary>
///     Extensions for symbols.
/// </summary>
internal static class SymbolExtensions
{
    public static AttributeData? GetAttribute(this ITypeSymbol self, string name) =>
        self.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToString() == name);

    public static bool HasAttribute(this ITypeSymbol self, string name) =>
        self.GetAttribute(name) is not null;

    public static string ToFullDisplayString(this ISymbol self) =>
        self.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public static string ToStringWithAttributes(this ISymbol self)
    {
        var attributes = self.GetAttributes();
        if (attributes.Length > 0)
            return "[" + string.Join(", ", attributes) + "] " + self;
        else
            return self.ToString();
    }
}