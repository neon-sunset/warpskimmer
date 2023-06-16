﻿namespace Feetlicker;

public readonly record struct Prefix(
    U8String Host,
    U8String? Nickname = null,
    U8String? Username = null)
{
    public static unsafe Prefix Parse(U8String source)
    {
        var nickname = default(U8String?);
        var username = default(U8String?);

        var (nick, rest) = source.SplitFirst((byte)'!');
        if (nick.Length > 0)
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