using Traits.Playground.Examples.Conversion;
using Traits.Playground.Examples.Defaults;
using Traits.Playground.Examples.Hashing;
using Traits.Playground.Examples.Nums;
using Traits.Playground.Examples.Sets;
using Traits.Playground.Examples.Sets.Extensions;

var def = Default.Of<string>();
var sum = new[] { 1, 2, 3 }.Fold();
var arr = Monoid.Zero<object[]>();

Console.WriteLine(def == "");
Console.WriteLine(sum);
Console.WriteLine(arr.Length);

Console.WriteLine(Sum(1, 5, 10));
Console.WriteLine(Sum(1.2, 3.4));
// Console.WriteLine(Sum("x", "y")); // Won't compile.

Console.WriteLine(Zero<int>());
// Console.WriteLine(Zero<float>()); // Won't compile.

Console.WriteLine(-Parse<int>("-1984"));
Console.WriteLine(-Parse<float>("1.2"));
Console.WriteLine(!Parse<bool>("true"));
// Console.WriteLine(-Parse<double>("3.4")); // Won't compile.

Console.WriteLine(Convert<int, string>("135"));
Console.WriteLine(Convert<float, string>("1.5"));
// Console.WriteLine(Convert<double, string>("1.5")); // Won't compile.
// Console.WriteLine(Convert<float, int>(1)); // Won't compile.

Console.WriteLine(new Collection<string>("1", "2", "3").To<int>());
// Console.WriteLine(new Collection<string>("1", "2", "3").To<short>()); // Won't compile.

static T Sum<[Num] T>(params T[] items)
{
    if (items.Length == 0)
        throw new ArgumentException("The array is empty");

    var sum = items[0];
    for (var i = 1; i < items.Length; ++i)
        sum = Num.Add(sum, items[i]);

    return sum;
}

static int Zero<[Default, Hash] T>() =>
    Hash.Of(Default.Of<T>());

static T Parse<[From<string>] T>(string text) =>
    From<string>.Into<T>(text);

static T Convert<[From(nameof(U))] T, U>(U input) =>
    From<U>.Into<T>(input);

class Collection<T>
{
    private readonly List<T> _items;

    public Collection(params T[] items)
        : this(items.ToList()) { }

    public Collection(List<T> items) =>
        _items = items;

    public Collection<U> To<[From(nameof(T))] U>() =>
        new(_items.Select(x => From<T>.Into<U>(x)).ToList());

    public override string ToString() =>
        "[" + string.Join(", ", _items) + "]";
}
