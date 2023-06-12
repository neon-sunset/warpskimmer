// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Feetlicker;
using U8Primitives;

// var nonmsgs = File
//     .ReadLines("data.txt")
//     .Where(line => !line.Contains("PRIVMSG"))
//     .ToArray();

// foreach (var line in nonmsgs.Take(100))
//     Console.WriteLine(line);

// Console.WriteLine($"nonmsg count: {nonmsgs.Length}");

BenchmarkRunner.Run<CommandBenchmark>();

[ShortRunJob, ShortRunJob(RuntimeMoniker.NativeAot80)]
// [MemoryDiagnoser, DisassemblyDiagnoser(maxDepth: 3)]
public class CommandBenchmark
{
    [Params("PRIVMSG", "CLEARCHAT #forsen", "USERNOTICE xdd")]
    public string Value = "";

    private U8String RawCommand;

    [GlobalSetup]
    public void Setup()
    {
        RawCommand = Value.ToU8String();
    }

    [Benchmark]
    public Command? Parse()
    {
        var bytes = RawCommand.AsSpan();
        return Command.Parse(ref bytes);
    }

    [Benchmark]
    public Command? Parse2()
    {
        var bytes = RawCommand.AsSpan();
        return Command.Parse2(ref bytes);
    }

    [Benchmark]
    public Command? ParseReference()
    {
        return RawCommand.StartsWith("PRIVMSG"u8) ? Command.Privmsg : null;
    }
}
