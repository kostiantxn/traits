using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Traits.Analyzers;
using Traits.Generators;
using Traits.Tests.Core.Sources;

namespace Traits.Tests.Core.Verifiers;

internal static class Verify
{
    private static readonly MetadataReference[] References =
    {
        MetadataReference.CreateFromFile(typeof(TraitAttribute).Assembly.Location),
    };

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, IEnumerable{DiagnosticResult})"/>
    public static Task Analyzer(
        DiagnosticDescriptor diagnostic,
        [InterpolatedStringHandlerArgument("diagnostic")] SourceInterpolatedStringHandler handler,
        params Source[] sources) =>
        Analyzer(sources.Append(handler.Source));

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, IEnumerable{DiagnosticResult})"/>
    public static Task Analyzer(
        DiagnosticDescriptor[] diagnostics,
        [InterpolatedStringHandlerArgument("diagnostics")] SourceInterpolatedStringHandler handler,
        params Source[] sources) =>
        Analyzer(sources.Append(handler.Source));

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, IEnumerable{DiagnosticResult})"/>
    public static Task Analyzer(
        Dictionary<string, DiagnosticDescriptor> diagnostics,
        [InterpolatedStringHandlerArgument("diagnostics")] SourceInterpolatedStringHandler handler,
        params Source[] sources) =>
        Analyzer(sources.Append(handler.Source));

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, IEnumerable{DiagnosticResult})"/>
    public static Task Analyzer(params Source[] sources) =>
        Analyzer(sources.AsEnumerable());

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, IEnumerable{DiagnosticResult})"/>
    public static Task Analyzer(IEnumerable<Source> sources) =>
        Analyzer(sources, Enumerable.Empty<DiagnosticResult>());

    /// <summary>
    ///     Runs the <see cref="TraitAnalyzer"/> and verifies reported diagnostics.
    /// </summary>
    public static Task Analyzer(IEnumerable<Source> sources, IEnumerable<DiagnosticResult> diagnostics)
    {
        var test = new CSharpAnalyzerTest<TraitAnalyzer, XUnitVerifier>
        {
            TestState = { ReferenceAssemblies = ReferenceAssemblies.Net.Net60 }
        };

        foreach (var source in sources)
        {
            if (source.Path is not null)
                test.TestState.Sources.Add((source.Path, source.Analyze));
            else
                test.TestState.Sources.Add(source.Analyze);

            var generator = Driver(source.Generate).GetRunResult();

            foreach (var generated in generator.GeneratedTrees)
                test.TestState.Sources.Add(generated.GetText());
        }

        test.TestState.AdditionalReferences.AddRange(References);
        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);

        return test.RunAsync();
    }

    /// <summary>
    ///     Creates a <see cref="CSharpGeneratorDriver"/> that run the <see cref="TraitGenerator"/>
    ///     on the provided source code.
    /// </summary>
    private static GeneratorDriver Driver(string source, string assembly = "Traits.Tests")
    {
        var tree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(assembly, new[] { tree }, References);

        return CSharpGeneratorDriver
            .Create(new TraitGenerator())
            .RunGenerators(compilation);
    }
}