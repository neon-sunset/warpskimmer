using U8Primitives.InteropServices;

namespace Warpskimmer;

public record Prefix
{
    readonly U8Source _source;
    readonly U8Range _host;
    readonly U8Range? _nickname;
    readonly U8Range? _username;

    internal Prefix(U8Source source, U8Range host, U8Range? nickname, U8Range? username)
    {
        _source = source;
        _host = host;
        _nickname = nickname;
        _username = username;
    }

    public U8String Host => U8Marshal.Slice(_source, _host);

    public U8String? Nickname
    {
        get => _nickname.HasValue ? U8Marshal.Slice(_source, _nickname.Value) : null;
    }

    public U8String? Username
    {
        get => _username.HasValue ? U8Marshal.Slice(_source, _username.Value) : null;
    }

    internal static Prefix? Parse(ref U8String source)
    {
        var prefixValue = source;
        if (prefixValue[0] == (byte)':')
        {
            (prefixValue, source) = U8Marshal
                .Slice(prefixValue, 1)
                .SplitFirst((byte)' ');

            var nickname = default(U8Range?);
            var username = default(U8Range?);

            var (nick, rest) = prefixValue.SplitFirst((byte)'!');
            if (!nick.IsEmpty)
            {
                nickname = nick.Range;
            }

            var (user, host) = rest.SplitFirst((byte)'@');
            if (host.IsEmpty)
            {
                host = user;
            }
            else
            {
                username = user.Range;
            }

            return new Prefix(prefixValue.Source, host.Range, nickname, username);
        }

        return null;
    }
}
