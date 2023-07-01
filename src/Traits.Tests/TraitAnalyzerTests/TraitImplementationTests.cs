using Traits.Analyzers;
using Traits.Tests.Core.Verifiers;

namespace Traits.Tests.TraitAnalyzerTests;

public class TraitImplementationTests
{
    [Fact]
    public async Task Error_WhenNotSealed()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Implementation.MustBeSealed,
            $$"""
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }

            class {{"IntHash"}} : IHash<int>
            {
                public int Of(int self) => self;
            }
            """);
    }

    [Fact]
    public async Task Error_WhenImplementsOtherInterfaces()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Implementation.MustNotImplementOtherInterfaces,
            $$"""
            using System;
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }

            sealed class {{"IntHash"}} : IHash<int>, ICloneable
            {
                public int Of(int self) => self;

                public object Clone() => this;
            }
            """);
    }

    [Fact]
    public async Task Error_WhenDoesNotHavePublicParameterlessConstructor()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Implementation.MustContainParameterlessConstructor,
            $$"""
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

            sealed class {{"ShortHash"}} : IHash<short>
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

            sealed class {{"LongHash"}} : IHash<long>
            {
                public LongHash(object something) { }

                public int Of(long self) => self.GetHashCode();
            }
            """);
    }

    [Fact]
    public async Task Error_WhenHasInconsistentAccessibility()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Implementation.MustBeAsAccessibleAsTrait,
            $$"""
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

            public sealed class {{"LongHash"}} : IHash<long>
            {
                public int Of(long self) => self.GetHashCode();
            }

            [Trait]
            public interface IDefault<S>
            {
                S Of();
            }

            internal sealed class {{"IntDefault"}} : IDefault<int>
            {
                public int Of() => 0;
            }

            public sealed class LongDefault : IDefault<long>
            {
                public long Of() => 0L;
            }
            """);
    }

    [Fact]
    public async Task Error_WhenNested()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Implementation.MustNotBeNested,
            $$"""
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }

            class Scope
            {
                internal sealed class {{"IntHash"}} : IHash<int>
                {
                    public int Of(int self) => self;
                }
            }
            """);
    }

    [Fact]
    public async Task Error_WhenConflictsWithAnotherImplementation()
    {
        // lang=C#
        await Verify.Analyzer(
            Diagnostics.Implementation.Conflict,
            $$"""
            using Traits;

            [Trait]
            interface IHash<S>
            {
                int Of(S self);
            }

            sealed class {{"Int1Hash"}} : IHash<int>
            {
                public int Of(int self) => self;
            }

            sealed class {{"Int2Hash"}} : IHash<int>
            {
                public int Of(int self) => self;
            }
            """);
    }

    [Fact]
    public async Task Noop_WhenCorrectlyDefined()
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
                public int Of(byte self) => self;
            }

            sealed class ShortHash : IHash<short>
            {
                public int Of(short self) => self;
            }

            sealed class IntHash : IHash<int>
            {
                public int Of(int self) => self;
            }

            sealed class ObjectHash : IHash<object>
            {
                public int Of(object self) => self.GetHashCode();
            }

            sealed class StringHash : IHash<string>
            {
                public int Of(string self) => self.GetHashCode();
            }
            """);
    }
}