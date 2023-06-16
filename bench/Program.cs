using BenchmarkDotNet.Running;

BenchmarkSwitcher
    .FromAssembly(typeof(Program).Assembly)
    .Run(args);

// var parsed = File
//     .ReadLines("data.txt")
//     .Select(x => x.ToU8String())
//     .Take(100)
//     .ToArray();

// foreach (var msg in parsed)
//     Console.WriteLine(Message.Parse(msg));