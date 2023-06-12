using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Feetlicker;

public record Message(
    Tag[]? Tags,
    Prefix? Pfx,
    Command Cmd,
    U8String? Chan,
    U8String? Params)
{
    // TODO: Stateful line reader/parser
    public static Message Parse(ReadOnlySpan<byte> line)
    {
        var source = line;

        var tags = Tag.ParseAll(ref source);
        var prefix = Prefix.Parse(ref source);
        var command = Command.Parse(ref source);

        throw new NotImplementedException();
    }
}
