using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Feetlicker.Benchmark;

[MemoryDiagnoser]
[SimpleJob, SimpleJob(RuntimeMoniker.NativeAot80)]
public class ParseMessage
{
    private U8String[] Lines = null!;

    [Params(1, 1000, 100_000)]
    public int Count;

    [GlobalSetup]
    public void Setup()
    {
        Lines = File
            .ReadLines("data.txt")
            .Select(x => x.ToU8String())
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
