using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace Feetlicker.Benchmark;


[ShortRunJob, ShortRunJob(RuntimeMoniker.NativeAot80)]
// [MemoryDiagnoser, DisassemblyDiagnoser(maxDepth: 3)]
public class ParseCommand
{
    [Params("PRIVMSG", "CLEARCHAT", "USERNOTICE")]
    public string Value = "";

    private U8String RawCommand;

    [GlobalSetup]
    public void Setup()
    {
        RawCommand = Value.ToU8String();
    }

    [Benchmark]
    public Command? Parse() => Command.Parse(RawCommand);

    [Benchmark]
    public Command? ParseReference()
    {
        return RawCommand.AsSpan() switch
        {
            var s when s.StartsWith("PRIVMSG"u8) => Command.Privmsg,
            var s when s.StartsWith("CLEARCHAT"u8) => Command.Clearchat,
            var s when s.StartsWith("USERNOTICE"u8) => Command.UserNotice,
            _ => null
        };
    }
}
