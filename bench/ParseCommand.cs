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
    public Command? Parse()
    {
        var local = RawCommand;
        return Command.Parse(ref local);
    }

    [Benchmark]
    public Command? ParseReference()
    {
        var local = RawCommand;
        return local switch
        {
            _ when local.Equals("PRIVMSG"u8) => Command.Privmsg,
            _ when local.Equals("CLEARCHAT"u8) => Command.Clearchat,
            _ when local.Equals("USERNOTICE"u8) => Command.UserNotice,
            _ => null
        };
    }
}
