using Microsoft.CodeAnalysis;

namespace Traits.Analyzers.Extensions;

/// <summary>
///     Extensions for symbols.
/// </summary>
internal static class SymbolExtensions
{
    public static AttributeData? GetAttribute(this ITypeSymbol self, ITypeSymbol type) =>
        self.GetAttributes().FirstOrDefault(
            x => x.AttributeClass is not null && SymbolEqualityComparer.Default.Equals(x.AttributeClass, type));

    public static AttributeData? GetAttribute(this ITypeSymbol self, string name) =>
        self.GetAttributes().FirstOrDefault(x => x.AttributeClass?.ToString() == name);

    public static bool HasAttribute(this ITypeSymbol self, ITypeSymbol type) =>
        self.GetAttribute(type) is not null;

    public static bool HasAttribute(this ITypeSymbol self, string name) =>
        self.GetAttribute(name) is not null;

    public static bool Inherits(this ITypeSymbol self, string name) =>
        self.ToString() == name || (self.BaseType is not null && self.BaseType.Inherits(name));

    public static INamedTypeSymbol Substitute(this INamedTypeSymbol self, ITypeSymbol parameter, ITypeSymbol argument)
    {
        if (!self.IsGenericType || self.IsUnboundGenericType)
            return self;

        var arguments = new ITypeSymbol[self.TypeArguments.Length];
        for (var i = 0; i < self.TypeArguments.Length; ++i)
            arguments[i] = self.TypeArguments[i].Substitute(parameter, argument);

        return self.OriginalDefinition.Construct(arguments);
    }

    public static ITypeSymbol Substitute(this ITypeSymbol self, ITypeSymbol parameter, ITypeSymbol argument)
    {
        if (SymbolEqualityComparer.Default.Equals(self, parameter))
            return argument;

        return self is INamedTypeSymbol named 
            ? named.Substitute(parameter, argument) 
            : self;
    }
}