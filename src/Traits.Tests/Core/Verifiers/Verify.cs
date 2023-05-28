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

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, DiagnosticResult[])"/>
    public static Task Analyzer(
        Source source,
        params DiagnosticResult[] diagnostics) =>
        Analyzer(new[] { source }, diagnostics);

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, DiagnosticResult[])"/>
    public static Task Analyzer(
        Source source0,
        Source source1,
        params DiagnosticResult[] diagnostics) =>
        Analyzer(new[] { source0, source1 }, diagnostics);

    /// <inheritdoc cref="Analyzer(IEnumerable{Source}, DiagnosticResult[])"/>
    public static Task Analyzer(
        Source source0,
        Source source1,
        Source source2,
        params DiagnosticResult[] diagnostics) =>
        Analyzer(new[] { source0, source1, source2 }, diagnostics);

    /// <summary>
    ///     Runs the <see cref="TraitAnalyzer"/> and verifies reported diagnostics.
    /// </summary>
    public static Task Analyzer(IEnumerable<Source> sources, params DiagnosticResult[] diagnostics)
    {
        var test = new CSharpAnalyzerTest<TraitAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60
            }
        };

        foreach (var source in sources)
        {
            if (source.Path is not null)
                test.TestState.Sources.Add((source.Path, source.Text));
            else
                test.TestState.Sources.Add(source.Text);

            var generator = Driver(source.Text).GetRunResult();

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