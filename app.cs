#:package Humanizer@2.*

using Humanizer;

var date = new DateTime(2020, 1, 1);

Console.WriteLine($"Hello, World! {date.Humanize()}");
