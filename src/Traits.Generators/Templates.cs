using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Traits.Generators.Extensions;

namespace Traits.Generators;

/// <summary>
///     Templates for files that 
/// </summary>
internal static class Templates
{
    public class File
    {
        /// <summary>
        ///     The path to the generated file.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     The generated source.
        /// </summary>
        public string Source { get; }

        public File(string name, string source) =>
            (Path, Source) = (name, source);

        public File(string name, StringBuilder source)
            : this(name, source.ToString()) { }
    }

    /// <summary>
    ///     Generates the static facade class for the specified trait.
    /// </summary>
    /// <example>
    ///     A static <c>Hash</c> class will be generated for the <c>IHash&lt;S&gt;</c> trait.
    /// </example>
    public static File Facade(INamedTypeSymbol type)
    {
        var source = new StringBuilder();

        if (!type.ContainingNamespace.IsGlobalNamespace)
            source
                .AppendLine($"namespace {type.ContainingNamespace};")
                .AppendLine();

        var name = NameWithoutLeadingI(type);
        var accessibility = SyntaxFacts.GetText(type.DeclaredAccessibility);

        // TODO: Copy type parameter constraints from the original trait definition.
        var self = type.TypeParameters.First();
        var generics = string.Join(", ", type.TypeParameters.Skip(1).Select(x => x.ToStringWithAttributes()));
        if (generics.Length > 0)
            generics = "<" + generics + ">";

        source.AppendLine(
            $$"""
            using System.Linq;

            /// <inheritdoc cref="{{Escape(type.ToFullDisplayString())}}"/>
            [global::Traits.ForAttribute(typeof({{type.ConstructUnboundGenericType().ToFullDisplayString()}}))]
            {{accessibility}} static class {{name}}{{generics}}
            {
            #pragma warning disable CS0649
            #pragma warning disable TR2001
                private static class Impl<{{self}}>
                {
                    public static {{type.ToFullDisplayString()}} Instance;
                }
            #pragma warning restore TR2001
            #pragma warning restore CS0649

                static {{name}}()
                {
                    var traits = global::System.AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Select(x => (Impl: x, Definition: Definition(x)))
                        .Where(x => x.Definition is not null);

                    global::System.Type Definition(global::System.Type type) =>
                        type.GetInterfaces()
                            .Where(x => x.IsGenericType)
                            .Where(x => x.GetGenericTypeDefinition() == typeof({{type.ConstructUnboundGenericType().ToFullDisplayString()}}))
                            {{Filter(tab: 16)}}.FirstOrDefault();

                    foreach (var trait in traits)
                    {
                        var self = trait.Definition.GenericTypeArguments.First();
                        var arguments = trait.Definition.GenericTypeArguments.Skip(1).Append(self).ToArray();
                        var impl = typeof(Impl<>).MakeGenericType(arguments);

                        var instance = impl.GetField(nameof(Impl<object>.Instance));
                        if (instance is null)
                            continue;

                        var ctor = trait.Impl.GetConstructor(global::System.Type.EmptyTypes);
                        if (ctor is null)
                            continue;

                        instance.SetValue(null, ctor.Invoke(null));
                    }
                }
            """);

        var methods = type.GetMembers().OfType<IMethodSymbol>()
            .Where(x => x.DeclaredAccessibility == Accessibility.Public)
            .Where(x => !x.IsStatic)
            .Select(Method);

        var body = string.Join("\n\n", methods);
        if (body.Length > 0)
            source.AppendLine().AppendLine(body);

        source.AppendLine(
            "}");

        return new File($"{name}.g.cs", source);

        string Method(IMethodSymbol method)
        {
            var attribute = name + "Attribute";

            if (!type.ContainingNamespace.IsGlobalNamespace)
                attribute = type.ContainingNamespace.ToFullDisplayString() + "." + attribute;
            else
                attribute = "global::" + attribute;

            if (type.TypeParameters.Length > 1)
                attribute = attribute + "(" + string.Join(", ", type.TypeParameters.Skip(1).Select(x => "nameof(" + x.Name + ")")) + ")";

            // TODO: Include attributes from the original methods.
            // TODO: Include generics from the original method (and not only the `self` parameter).
            // TODO: Include other `self` type parameter attributes.
            // TODO: Include `self` type parameter constraints.
            return
                $"""
                    /// <inheritdoc cref="{Escape(type.ToFullDisplayString())}.{Escape(method.ToFullDisplayString())}"/>
                    [global::System.Runtime.CompilerServices.MethodImplAttribute(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
                    public static {method.ReturnType} {method.Name}<[{attribute}] {self}>({string.Join(", ", method.Parameters.Select(x => x.ToParameterString()))}) =>
                        Impl<{self.ToFullDisplayString()}>.Instance.{method.Name}({string.Join(", ", method.Parameters.Select(x => x.ToArgumentString()))});
                """;
        }

        string Filter(int tab) =>
            string.Join(
                string.Empty,
                type.TypeParameters.Skip(1).Select((x, i) => 
                    $".Where(x => x.GenericTypeArguments[{i + 1}] == typeof({x.Name}))\n" + new string(' ', tab)));
    }

    /// <summary>
    ///     Generates a trait constraint attribute for the specified trait.
    /// </summary>
    /// <example>
    ///     A <c>HashAttribute</c> trait constraint will be generated for the <c>IHash&lt;S&gt;</c> trait.
    /// </example>
    public static File Attribute(INamedTypeSymbol type)
    {
        var source = new StringBuilder();

        if (!type.ContainingNamespace.IsGlobalNamespace)
            source
                .AppendLine($"namespace {type.ContainingNamespace};")
                .AppendLine();

        var name = NameWithoutLeadingI(type);
        var accessibility = SyntaxFacts.GetText(type.DeclaredAccessibility);

        source.AppendLine(
            $$"""
            /// <summary>
            ///     Requires the marked type parameter to implement the <see cref="{{Escape(type.ToFullDisplayString())}}"/> trait.
            /// </summary>
            [global::Traits.ForAttribute(typeof({{type.ConstructUnboundGenericType().ToFullDisplayString()}}))]
            {{accessibility}} class {{name}}Attribute : global::Traits.ConstraintAttribute
            {
            """);

        // Append a constructor that can be used with the `nameof` operator.
        var parameters = string.Join(", ", type.TypeParameters.Skip(1).Select(x => "string " + CamelCase(x)));
        if (parameters.Length > 0)
            source.AppendLine(
                $$"""
                    public {{name}}Attribute({{parameters}})
                    {
                    }
                """);

        source.AppendLine(
            "}");

        // Append a generic version of the attribute if the trait is generic.
        var generics = string.Join(", ", type.TypeParameters.Skip(1).Select(x => x.ToStringWithAttributes()));
        if (generics.Length > 0)
            source.AppendLine().AppendLine(
                $$"""
                /// <summary>
                ///     Requires the marked type parameter to implement the <see cref="{{Escape(type.ToFullDisplayString())}}"/> trait.
                /// </summary>
                [global::Traits.ForAttribute(typeof({{type.ConstructUnboundGenericType().ToFullDisplayString()}}))]
                {{accessibility}} class {{name}}Attribute<{{generics}}> : global::Traits.ConstraintAttribute
                {
                }
                """);

        return new File($"{name}Attribute.g.cs", source);
    }

    /// <summary>
    ///     Returns the name of the interface without the leading <c>I</c> character.
    /// </summary>
    private static string NameWithoutLeadingI(ITypeSymbol type)
    {
        var name = type.Name;
        if (name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
            return name.Substring(1);
        else 
            return name;
    }

    /// <summary>
    ///     Convert the name of the provided type to camel case.
    /// </summary>
    private static string CamelCase(ITypeSymbol type) =>
        char.ToLower(type.Name[0]) + type.Name.Substring(1);

    /// <summary>
    ///     Escapes the specified text to allow it to be used in XML documentation comments.
    /// </summary>
    /// <remarks>
    ///     Replaces <c>&lt;</c> with <c>{</c> and <c>&gt;</c> with <c>}</c>.
    /// </remarks>
    private static string Escape(string text) =>
        text.Replace('<', '{').Replace('>', '}');
}