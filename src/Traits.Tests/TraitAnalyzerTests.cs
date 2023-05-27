using Microsoft.CodeAnalysis.Testing;
using Traits.Analyzers;
using Traits.Tests.Verifiers;

namespace Traits.Tests;

public class TraitAnalyzerTests
{
    [Fact]
    public async Task EmitsError_WhenTraitInterfaceIsNotGeneric()
    {
        await Verify.Source(
            """
            using Traits;

            [Trait]
            interface IHash
            {
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Trait.MustHaveAtLeastOneGenericParameter.Id)
                .WithLocation(line: 4, column: 11));
    }

    [Fact]
    public async Task EmitsError_WhenTraitInterfaceExtendsOtherInterfaces()
    {
        await Verify.Source(
            """
            using System;
            using Traits;

            [Trait]
            interface IHash<S> : ICloneable
            {
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Trait.ShouldNotExtendOtherInterfaces.Id)
                .WithLocation(line: 5, column: 20));
    }
}