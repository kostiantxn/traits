using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

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

    public static string ToStringWithAttributes(this ISymbol self) =>
        self.ToAttributesString() + self;

    public static string ToParameterString(this IParameterSymbol self)
    {
        var text = new StringBuilder();

        text.Append(self.ToAttributesString())
            .Append(self.IsParams ? "params " : string.Empty)
            .Append(self.RefKind.ToDisplayString())
            .Append(self.Type.ToFullDisplayString())
            .Append(' ')
            .Append(self.Name);

        if (self.HasExplicitDefaultValue)
            text.Append(" = ")
                .Append(self.ExplicitDefaultValue.ToReprString(self.Type));

        return text.ToString();
    }

    public static string ToArgumentString(this IParameterSymbol self) =>
        self.RefKind.ToDisplayString() + self.Name;

    private static string ToAttributesString(this ISymbol self)
    {
        var attributes = self.GetAttributes();
        if (attributes.Length > 0)
            return "[" + string.Join(", ", attributes) + "] ";

        return string.Empty;
    }

    private static string ToDisplayString(this RefKind kind) =>
        kind switch
        {
            RefKind.In => "in ",
            RefKind.Out => "out ",
            RefKind.Ref => "ref ",
            _ => string.Empty,
        };

    private static string ToReprString(this object? self, ITypeSymbol type)
    {
        if (self is null)
            return "default";

        if (type.IsValueType && 
            type is INamedTypeSymbol { IsGenericType: true } named && 
            type.OriginalDefinition.ToFullDisplayString() == "global::System.Nullable<T>")
        {
            var underlying = named.TypeArguments.First();

            return self.ToReprString(underlying);
        }

        if (type.TypeKind == TypeKind.Enum)
            return $"({type.ToFullDisplayString()}) {self}";

        var text = SymbolDisplay.FormatPrimitive(self, quoteStrings: true, useHexadecimalNumbers: false);

        if (self is float)
            return text + "F";
        if (self is decimal)
            return text + "M";

        return text;
    }
}