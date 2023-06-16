using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Feetlicker.Benchmark;

[MemoryDiagnoser]
[SimpleJob, SimpleJob(RuntimeMoniker.NativeAot80)]
public class ParseMessage
{
    private U8String[] Lines = null!;

    [GlobalSetup]
    public void Setup()
    {
        Lines = File
            .ReadLines("data.txt")
            .Select(x => x.ToU8String())
            .Take(1000)
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
