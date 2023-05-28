using Microsoft.CodeAnalysis.Testing;
using Traits.Analyzers;
using Traits.Tests.Core.Verifiers;

namespace Traits.Tests.TraitAnalyzerTests;

public class TraitImplementationTests
{
    [Fact]
    public async Task EmitsError_WhenNotSealed()
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

            class IntHash : IHash<int>
            {
                public int Of(int self) => self;
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Implementation.MustBeSealed.Id)
                .WithLocation(9, 7));
    }

    [Fact]
    public async Task EmitsError_WhenImplementsOtherInterfaces()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            using System;
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }

            sealed class IntHash : IHash<int>, ICloneable
            {
                public int Of(int self) => self;

                public object Clone() => this;
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Implementation.MustNotImplementOtherInterfaces.Id)
                .WithLocation(10, 14));
    }

    [Fact]
    public async Task EmitsError_WhenDoesNotHavePublicParameterlessConstructor()
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

            sealed class ByteHash : IHash<byte>
            {
                public ByteHash() { }

                public int Of(byte self) => self;
            }

            sealed class ShortHash : IHash<short>
            {
                private ShortHash() { }

                public int Of(short self) => self;
            }

            sealed class IntHash : IHash<int>
            {
                public IntHash() { }
                public IntHash(object something) { }

                public int Of(int self) => self;
            }

            sealed class LongHash : IHash<long>
            {
                public LongHash(object something) { }

                public int Of(long self) => self.GetHashCode();
            }
            """,
            DiagnosticResult // `ShortHash`
                .CompilerError(Diagnostics.Implementation.MustContainParameterlessConstructor.Id)
                .WithLocation(16, 14),
            DiagnosticResult // `LongHash`
                .CompilerError(Diagnostics.Implementation.MustContainParameterlessConstructor.Id)
                .WithLocation(31, 14));
    }

    [Fact]
    public async Task EmitsError_WhenHasDifferentAccessibility()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            using Traits;

            [Trait]
            internal interface IHash<S>
            {
                int Of(S self);
            }

            internal sealed class IntHash : IHash<int>
            {
                public int Of(int self) => self;
            }

            public sealed class LongHash : IHash<long>
            {
                public int Of(long self) => self.GetHashCode();
            }

            [Trait]
            public interface IDefault<S>
            {
                S Of();
            }

            internal sealed class IntDefault : IDefault<int>
            {
                public int Of() => 0;
            }

            public sealed class LongDefault : IDefault<long>
            {
                public long Of() => 0L;
            }
            """,
            DiagnosticResult // `LongHash`
                .CompilerError(Diagnostics.Implementation.MustBeAsAccessibleAsTrait.Id)
                .WithLocation(14, 21),
            DiagnosticResult // `IntDefault`
                .CompilerError(Diagnostics.Implementation.MustBeAsAccessibleAsTrait.Id)
                .WithLocation(25, 23));
    }

    [Fact]
    public async Task EmitsError_WhenNested()
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

            class Scope
            {
                internal sealed class IntHash : IHash<int>
                {
                    public int Of(int self) => self;
                }
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Implementation.MustNotBeNested.Id)
                .WithLocation(11, 27));
    }

    [Fact]
    public async Task EmitsError_WhenConflictsWithAnotherImplementation()
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

            sealed class Int1Hash : IHash<int>
            {
                public int Of(int self) => self;
            }

            sealed class Int2Hash : IHash<int>
            {
                public int Of(int self) => self;
            }
            """,
            DiagnosticResult
                .CompilerError(Diagnostics.Implementation.Conflict.Id)
                .WithLocation(9, 14),
            DiagnosticResult
                .CompilerError(Diagnostics.Implementation.Conflict.Id)
                .WithLocation(14, 14));
    }

    [Fact]
    public async Task EmitsNothing_WhenCorrectlyDefined()
    {
        // lang=C#
        await Verify.Analyzer(
            """
            using Traits;

            [Trait]
            public interface IHash<S>
            {
                int Of(S self);
            }

            public sealed class ByteHash : IHash<byte>
            {
                public int Of(byte self) => self;
            }

            public sealed class ShortHash : IHash<short>
            {
                public int Of(short self) => self;
            }

            public sealed class IntHash : IHash<int>
            {
                public int Of(int self) => self;
            }

            public sealed class ObjectHash : IHash<object>
            {
                public int Of(object self) => self.GetHashCode();
            }

            public sealed class StringHash : IHash<string>
            {
                public int Of(string self) => self.GetHashCode();
            }
            """);
    }
}