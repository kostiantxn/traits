using Traits.Playground.Examples.Defaults;
using Traits.Playground.Examples.Sets.Extensions;

var def = Default.Of<string>();
var sum = new[] { 1, 2, 3 }.Fold();

Console.WriteLine(def == "");
Console.WriteLine(sum);
