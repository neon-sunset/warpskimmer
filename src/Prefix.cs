using U8Primitives.InteropServices;

namespace Warpskimmer;

public record Prefix(
    U8String Host,
    U8String? Nickname = null,
    U8String? Username = null)
{
    public static Prefix? Parse(ref U8String source)
    {
        var prefixValue = source;
        if (prefixValue[0] != (byte)':')
        {
            return null;
        }

        (prefixValue, source) = U8Marshal
            .Slice(prefixValue, 1)
            .SplitFirst((byte)' ');

        var nickname = default(U8String?);
        var username = default(U8String?);

        var (nick, rest) = prefixValue.SplitFirst((byte)'!');
        if (!nick.IsEmpty)
        {
            nickname = nick;
        }

        var (user, host) = rest.SplitFirst((byte)'@');
        if (host.IsEmpty)
        {
            host = user;
        }
        else
        {
            username = user;
        }

        return new Prefix(host, nickname, username);
    }
}
