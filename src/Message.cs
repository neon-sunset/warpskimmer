using U8Primitives.InteropServices;

namespace Feetlicker;

public record Message(
    Tag[]? Tags,
    Prefix? Prefix,
    Command Command,
    U8String? Channel,
    U8String? Parameters)
{
    public static Message Parse(U8String line)
    {
        var tags = Tag.ParseAll(ref line);
        var prefix = Feetlicker.Prefix.Parse(ref line);
        var command = Command.Parse(ref line);
        var channel = ParseChannel(ref line);
        var parameters = line switch
        {
            [(byte)':', ..] => U8Marshal.Slice(line, 1),
            { Length: > 0 } => line,
            _ => default(U8String?)
        };

        return new Message(tags, prefix, command, channel, parameters);
    }

    private static U8String? ParseChannel(ref U8String line)
    {
        var channel = default(U8String?);
        var deref = line;
        if (deref[0] is (byte)'#')
        {
            (channel, line) = U8Marshal
                .Slice(deref, 1)
                .SplitFirst((byte)' ');
        }

        return channel;
    }
}
