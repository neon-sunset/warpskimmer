using BenchmarkDotNet.Running;
using Feetlicker;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args);
    }
}

// var lines = File
//     .ReadLines("data.txt")
//     .Select(x => x.ToU8String())
//     .ToArray();

// foreach (var i in 0..100)
// {
//     foreach (var line in lines)
//     {
//         _ = Message.Parse(line);
//     }
// }
