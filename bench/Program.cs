using BenchmarkDotNet.Running;
using Feetlicker;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

// var lines = File
//     .ReadLines("data.txt")
//     .Take(1000)
//     .Select(x => x.ToU8String())
//     .ToArray();

// foreach (var line in lines)
// {
//     var msg = Message.Parse(line);
//     Console.WriteLine(msg);
// }
// foreach (var i in 0..100)
// {
//     foreach (var line in lines)
//     {
//         _ = Message.Parse(line);
//     }
// }
