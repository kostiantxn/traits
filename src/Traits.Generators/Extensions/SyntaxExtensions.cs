using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Traits.Generators.Extensions;

/// <summary>
///     Extensions for syntax nodes.
/// </summary>
public static class SyntaxExtensions
{
    public static bool HasAttribute(this MemberDeclarationSyntax self, string name) =>
        self.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.HasAttribute(name));

    public static bool HasAttribute(this AttributeSyntax self, string name) =>
        self.Name.ToString() == name || self.Name + "Attribute" == name;
}