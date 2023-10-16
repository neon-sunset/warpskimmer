using System.Diagnostics;
using Warpskimmer.Benchmark;

var bench = new ParseMessage { Count = 100_000 };

Console.WriteLine("Setup...\n");
bench.Setup();

Console.WriteLine("Warmup...\n");
for (var i = 0; i < 100; i++)
{
    bench.Parse();
}

Console.WriteLine("Benchmark...\n");
for (var i = 0; i < 10; i++)
{
    var timestamp = Stopwatch.GetTimestamp();

    for (var j = 0; j < 100; j++)
    {
        bench.Parse();
    }

    var elapsed = Stopwatch.GetElapsedTime(timestamp);
    var perIteration = elapsed.TotalMilliseconds / 100;

    Console.WriteLine($"Elapsed: {elapsed} per iteration: {perIteration:##.##}ms");
}
