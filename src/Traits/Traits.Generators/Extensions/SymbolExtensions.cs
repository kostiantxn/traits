using Microsoft.CodeAnalysis;

namespace Traits.Generators.Extensions;

/// <summary>
///     Extensions for symbols.
/// </summary>
public static class SymbolExtensions
{
    public static AttributeData? GetAttribute(this ITypeSymbol self, string name) =>
        self.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToString() == name);

    public static bool HasAttribute(this ITypeSymbol self, string name) =>
        self.GetAttribute(name) is not null;
}