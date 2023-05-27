using Microsoft.CodeAnalysis.Testing;
using Traits.Analyzers;
using Traits.Tests.Verifiers;

namespace Traits.Tests;

public class TraitAnalyzerTests
{
    [Fact]
    public async Task EmitsError_WhenTraitInterfaceIsNotGeneric()
    {
        await Verify.Traits(
            """
            using Traits;

            [Trait]
            interface IHash
            {
                int Of(object self);
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Trait.MustHaveAtLeastOneGenericParameter.Id)
                .WithLocation(line: 4, column: 11));
    }

    [Fact]
    public async Task EmitsError_WhenTraitInterfaceExtendsOtherInterfaces()
    {
        await Verify.Traits(
            """
            using System;
            using Traits;

            [Trait]
            interface IHash<S> : ICloneable
            {
                int Of(S self);
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Trait.ShouldNotExtendOtherInterfaces.Id)
                .WithLocation(line: 5, column: 20));
    }

    [Fact]
    public async Task EmitsNothing_WhenTraitInterfaceIsCorrect()
    {
        await Verify.Traits(
            """
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }
            """);
    }

    [Fact]
    public async Task EmitsNothing_WhenInterfaceIsNotMarkedAsTrait()
    {
        await Verify.Traits(
            """
            using System;
            using Traits;

            interface IHash : ICloneable
            {
                int Of(object self);
            }
            """);
    }
}