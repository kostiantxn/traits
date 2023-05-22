using Traits.Playground.Examples.Defaults;
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

static T Sum<[Num] T>(params T[] items)
{
    if (items.Length == 0)
        throw new ArgumentException("The array is empty");

    var sum = items[0];
    for (var i = 1; i < items.Length; ++i)
        sum = Num.Add(sum, items[i]);

    return sum;
}