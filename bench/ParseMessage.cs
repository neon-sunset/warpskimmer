using BenchmarkDotNet.Attributes;

using U8.IO;

namespace Warpskimmer.Benchmark;

[MemoryDiagnoser]
public class ParseMessage
{
    private U8String[] Lines = null!;

    [Params(1, 1000, 100_000, int.MaxValue)]
    public int Count;

    [GlobalSetup]
    public void Setup()
    {
        Lines = U8File
            .ReadLines("data.txt")
            .Take(Count)
            .ToArray();
    }

    [Benchmark]
    public void Parse()
    {
        foreach (var line in Lines)
        {
            _ = Message.Parse(line);
        }
    }
}
