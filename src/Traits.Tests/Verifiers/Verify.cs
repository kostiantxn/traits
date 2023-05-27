﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Traits.Analyzers;

namespace Traits.Tests.Verifiers;

internal static class Verify
{
    /// <summary>
    ///     Runs the <see cref="TraitAnalyzer"/> and verifies reported diagnostics.
    /// </summary>
    public static Task Traits(string source, params DiagnosticResult[] diagnostics)
    {
        var test = new CSharpAnalyzerTest<TraitAnalyzer, XUnitVerifier>
        {
            TestState =
            {
                Sources = { source },
                AdditionalReferences =
                {
                    MetadataReference.CreateFromFile(typeof(TraitAttribute).Assembly.Location),
                },
                ReferenceAssemblies = ReferenceAssemblies.Net.Net60
            }
        };

        test.TestState.ExpectedDiagnostics.AddRange(diagnostics);

        return test.RunAsync();
    }
}