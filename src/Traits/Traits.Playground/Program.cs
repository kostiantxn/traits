using Traits.Playground.Examples.Defaults;
using Traits.Playground.Examples.Sets;
using Traits.Playground.Examples.Sets.Extensions;

var def = Default.Of<string>();
var sum = new[] { 1, 2, 3 }.Fold();
var arr = Monoid.Zero<object[]>();

Console.WriteLine(def == "");
Console.WriteLine(sum);
Console.WriteLine(arr.Length);
