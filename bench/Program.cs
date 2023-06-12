// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Feetlicker;

BenchmarkRunner.Run<CommandBenchmark>();

[ShortRunJob]
[MemoryDiagnoser, DisassemblyDiagnoser(maxDepth: 3)]
public class CommandBenchmark
{
    public byte[] Value = "PRIVMSG"u8.ToArray();

    [Benchmark]
    public Command? Parse()
    {
        var src = (ReadOnlySpan<byte>)Value.AsSpan();
        return Command.Parse(ref src);
    }
}
