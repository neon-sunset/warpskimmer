using BenchmarkDotNet.Attributes;

namespace Warpskimmer.Benchmark;

[MemoryDiagnoser]
public class ParseMessage
{
    private U8String[] Lines = null!;

    [Params(1, 1000, 100_000)]
    public int Count;

    [GlobalSetup]
    public void Setup()
    {
        using var file = File.OpenHandle("data.txt");

        Lines = U8String
            .Read(file)
            .Lines
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
