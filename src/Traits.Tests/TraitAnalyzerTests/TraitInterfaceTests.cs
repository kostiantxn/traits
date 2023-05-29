using Traits.Analyzers;
using Traits.Tests.Core.Verifiers;

namespace Traits.Tests.TraitAnalyzerTests;

public class TraitInterfaceTests
{
    [Fact]
    public async Task EmitsError_WhenNotGeneric()
    {
        // lang=C#
         await Verify.Analyzer(
             Diagnostics.Trait.MustHaveAtLeastOneGenericParameter,
             $$"""
             using Traits;

             [Trait]
             interface {{"IHash"}}
             {
                 int Of(object self);
             }
             """);
    }

    [Fact]
    public async Task EmitsError_WhenExtendsOtherInterfaces()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Trait.ShouldNotExtendOtherInterfaces,
            $$"""
            using System;
            using Traits;

            [Trait]
            interface IHash<S> {{": ICloneable"}}
            {
                int Of(S self);
            }
            """);
    }

    [Fact]
    public async Task EmitsNothing_WhenCorrectlyDefined()
    {
        // lang=C#
        await Verify.Analyzer(
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
    public async Task EmitsNothing_WhenNotMarkedAsTrait()
    {
        // lang=C#
        await Verify.Analyzer(
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