using U8Primitives.Unsafe;

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
            [(byte)':', ..] => U8Marshal.Substring(line, 1),
            { Length: > 0 } => line,
            _ => default(U8String?)
        };

        return new Message(tags, prefix, command, channel, parameters);
    }

    private static U8String? ParseChannel(ref U8String line)
    {
        var channel = default(U8String?);
        if (line is [(byte)'#', ..var channelValue])
        {
            var (channelName, rest) = channelValue.SplitFirst((byte)' ');
            channel = channelName;
            line = rest;
        }

        return channel;
    }
}
