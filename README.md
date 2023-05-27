# Traits

Roslyn-powered traits implementation for C#.

## Usage

### Defining a Trait

Consider the following definition of a trait for hashable types:

```csharp
[Trait]
interface IHash<S>
{
    int Of(S self);
}
```

The interface above is marked with the `[Trait]` attribute, which is the main type defined in the library.
It is used to indicate that the interface serves as the definition of a trait, and it tells the `TraitGenerator` to produce two auxiliary classes for working with the trait:
1. It first generates the static `Hash` class containing the same set of methods as the `IHash<S>` interface, which can be called for types that _implement_ the trait.
2. It also generates the `[Hash]` attribute, which can be applied to a generic type parameter to indicate that the parameter should allow only those types that implement the trait.

> **Note**: A trait interface must always contain at least one generic type parameter, the _self_ parameter, which denotes the type that implements the trait.
> The `TraitAnalyzer` will issue an error if you forget to add it to your trait.

### Implementing a Trait

We can implement the `IHash<S>` trait for different types by creating sealed classes that implement the interface.
For example, the following class implements the trait for the type `int`:

```csharp
sealed class IntHash : IHash<int>
{
    public int Of(int self) =>
        self;
}
```

This implementation can now be used via the static `Hash` class:

```csharp
var hash = Hash.Of(42);
```

The `Hash.Of` method automatically resolves the corresponding `IHash<S>` implementation based on the type of the provided value.
In the example above, the value is of type `int`, which resolves to the `IntHash` implementation of the trait.
Similarly, we can implement the trait for other types:

```csharp
record struct Point(int X, int Y);

sealed class PointHash : IHash<Point>
{
    public int Of(Point self) =>
        HashCode.Combine(self.X, self.Y);
}
```

We can then use the implementation:

```csharp
var hash = Hash.Of(new Point(X: 4, Y: 2));
```

> **Note**: Duplicate or conflicting trait implementations are not allowed, and the `TraitAnalyzer` will warn you about that.

You should note that you _cannot_ call the `Hash.Of` method with a value of a type, which does not implement the `IHash<S>` trait.
The `TraitAnalyzer` will issue an error if you attempt to pass an argument of such a type into the method.
This also includes generic type parameters:

> **Warning**: The following snippet _will not_ compile.
```csharp
int Bucket<T>(T item, int size) =>
    Hash.Of(item) % size;
```

### Require `T` to Implement a Trait

So, how can we use traits with values of generic types?
That's where the `[Hash]` attribute comes to the rescue.
In order to call the `Hash.Of` method with values of a generic type `T`, we must mark the type parameter with the automatically generated `[Hash]` attribute.
This attribute indicates that only those types that implement the `IHash<S>` trait should be used as type parameter `T`.
By using this attribute, we can now make the `Bucket` method compile:
```csharp
int Bucket<[Hash] T>(T item, int size) =>
    Hash.Of(item) % size;
```
And then we can pass any value of a type that implements the `IHash<S>` trait to the method:
```csharp
var bucket = Bucket(new Point(X: 4, Y: 2), size);
```
> **Note**: You can specify as many trait constraint attributes as you want!
> For example, `[Hash, Default] T` requires types used as the type parameter `T` to implement both the `IHash<S>` and `IDefault<S>` traits.

Trait constraint attributes can also be used to require all implementations of one trait to implement another trait.
Consider the following _semigroup_ trait, which defines a binary associative operation on a type:
```csharp
[Trait]
interface ISemigroup<S>
{
    S Dot(S x, S y);
}
```
We can now introduce the _monoid_ trait, which is a semigroup with a neutral element, as follows:
```csharp
[Trait]
interface IMonoid<[Semigroup] S>
{
    S Zero();
}
```
The definition of the `IMonoid<S>` trait requires all implementors to also implement the `ISemigroup<S>` trait.
Otherwise, if `ISemigroup<S>` is not implemented, an error will be issued by the `TraitAnalyzer`.

### Defining a Generic Trait

Trait interfaces can also be generic.
Consider the following trait, which defines a conversion between two types:
```csharp
[Trait]
interface IFrom<S, T>
{
    S Into(T value);
}
```
We can now add an implementation of this trait, which parses an `int` from a `string`:
```csharp
sealed class IntFromString : IFrom<int, string>
{
    public int Into(string value) =>
        int.Parse(value);
}
```

> **Note**: A single type can have multiple implementations of a generic trait.
> For example, we can define implementations of `IFrom<int, string>` and `IFrom<int, char>` to parse `int` from `string` or `char`.

This trait can then be used like so:
```csharp
T Parse<[From<string>] T>(string text) =>
    From<string>.Into<T>(text);
```

There is another way to require a type parameter to implement a generic trait by using the `nameof` operator.
Consider the following (slightly more complex) example:
```csharp
T Whatever<[From(nameof(U))] T, [Monoid] U>() =>
    From<U>.Into<T>(Monoid.Zero<U>());
```
In this example, we require the type `T` to define the `U` → `T` conversion by implementing the `IFrom<S, T>` trait, and we also require the type `U` to implement the `IMonoid<S>` trait.
As you can see, the `nameof` operator is especially useful here as it is impossible to use generic type parameters as type arguments of attributes.

### More Examples

For more examples, please refer to the [`samples`](samples) directory.

## License

The project is licensed under the [MIT](LICENSE) license.
