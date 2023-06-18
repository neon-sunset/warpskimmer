using CommunityToolkit.Diagnostics;

namespace Feetlicker;

public readonly record struct Command(
    CommandKey Key,
    U8String Value)
{
    private static ReadOnlySpan<byte> PRIVMSG => "PRIVMSG"u8;
    private static ReadOnlySpan<byte> CLEARCHAT => "CLEARCHAT"u8;

    public static readonly Command Ping =            new(CommandKey.Ping,            "PING"u8.ToU8String());
    public static readonly Command Pong =            new(CommandKey.Pong,            "PONG"u8.ToU8String());
    public static readonly Command Join =            new(CommandKey.Join,            "JOIN"u8.ToU8String());
    public static readonly Command Part =            new(CommandKey.Part,            "PART"u8.ToU8String());
    public static readonly Command Privmsg =         new(CommandKey.Privmsg,         PRIVMSG.ToU8String());
    public static readonly Command Whisper =         new(CommandKey.Whisper,         "WHISPER"u8.ToU8String());
    public static readonly Command Clearchat =       new(CommandKey.Clearchat,       CLEARCHAT.ToU8String());
    public static readonly Command Clearmsg =        new(CommandKey.Clearmsg,        "CLEARMSG"u8.ToU8String());
    public static readonly Command GlobalUserState = new(CommandKey.GlobalUserState, "GLOBALUSERSTATE"u8.ToU8String());
    public static readonly Command HostTarget =      new(CommandKey.HostTarget,      "HOSTTARGET"u8.ToU8String());
    public static readonly Command Notice =          new(CommandKey.Notice,          "NOTICE"u8.ToU8String());
    public static readonly Command Reconnect =       new(CommandKey.Reconnect,       "RECONNECT"u8.ToU8String());
    public static readonly Command RoomState =       new(CommandKey.RoomState,       "ROOMSTATE"u8.ToU8String());
    public static readonly Command UserNotice =      new(CommandKey.UserNotice,      "USERNOTICE"u8.ToU8String());
    public static readonly Command UserState =       new(CommandKey.UserState,       "USERSTATE"u8.ToU8String());
    public static readonly Command Capability =      new(CommandKey.Capability,      "CAP"u8.ToU8String());
    public static readonly Command RplWelcome =      new(CommandKey.RplWelcome,      "001"u8.ToU8String());
    public static readonly Command RplYourHost =     new(CommandKey.RplYourHost,     "002"u8.ToU8String());
    public static readonly Command RplCreated =      new(CommandKey.RplCreated,      "003"u8.ToU8String());
    public static readonly Command RplMyInfo =       new(CommandKey.RplMyInfo,       "004"u8.ToU8String());
    public static readonly Command RplNamReply =     new(CommandKey.RplNamReply,   "353"u8.ToU8String());
    public static readonly Command RplEndOfNames =   new(CommandKey.RplEndOfNames,   "366"u8.ToU8String());
    public static readonly Command RplMotd =         new(CommandKey.RplMotd,         "372"u8.ToU8String());
    public static readonly Command RplMotdStart =    new(CommandKey.RplMotdStart,    "375"u8.ToU8String());
    public static readonly Command RplEndOfMotd =    new(CommandKey.RplEndOfMotd,    "376"u8.ToU8String());

    public static Command Parse(ref U8String source)
    {
        // Does not handle 'CAP REQ'. What do?
        (var commandValue, source) = source.SplitFirst((byte)' ');

        return commandValue switch
        {
            _ when commandValue == PRIVMSG => Privmsg,
            _ when commandValue == CLEARCHAT => Clearchat,
            _ => ParseSlow(commandValue)
        };
    }

    public static Command ParseSlow(ReadOnlySpan<byte> source)
    {
        var rest = source[1..];
        return source[0] switch
        {
            (byte)'P' => rest switch
            {
                _ when rest.SequenceEqual("ING"U8) => Ping,
                _ when rest.SequenceEqual("ONG"U8) => Pong,
                _ when rest.SequenceEqual("ART"U8) => Part,
                _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
            },
            (byte)'J' when rest.SequenceEqual("OIN"u8) => Join,
            (byte)'W' when rest.SequenceEqual("HISPER"u8) => Whisper,
            (byte)'C' => rest switch
            {
                _ when rest.SequenceEqual("LEARMSG"u8) => Clearmsg,
                _ when rest.SequenceEqual("AP"u8) => Capability,
                _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
            },
            (byte)'G' when rest.SequenceEqual("LOBALUSERSTATE"u8) => GlobalUserState,
            (byte)'H' when rest.SequenceEqual("OSTTARGET"u8) => HostTarget,
            (byte)'N' when rest.SequenceEqual("OTICE"u8) => Notice,
            (byte)'R' => rest switch
            {
                _ when rest.SequenceEqual("ECONNECT"u8) => Reconnect,
                _ when rest.SequenceEqual("OOMSTATE"u8) => RoomState,
                _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
            },
            (byte)'U' => rest switch
            {
                _ when rest.SequenceEqual("SERNOTICE"u8) => UserNotice,
                _ when rest.SequenceEqual("SERSTATE"u8) => UserState,
                _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
            },
            (byte)'0' => rest switch
            {
                _ when rest.SequenceEqual("01"u8) => RplWelcome,
                _ when rest.SequenceEqual("02"u8) => RplYourHost,
                _ when rest.SequenceEqual("03"u8) => RplCreated,
                _ when rest.SequenceEqual("04"u8) => RplMyInfo,
                _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
            },
            (byte)'3' => rest switch
            {
                _ when rest.SequenceEqual("53"u8) => RplNamReply,
                _ when rest.SequenceEqual("66"u8) => RplEndOfNames,
                _ when rest.SequenceEqual("72"u8) => RplMotd,
                _ when rest.SequenceEqual("75"u8) => RplMotdStart,
                _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
            },
            _ => ThrowHelper.ThrowArgumentException<Command>(nameof(source))
        };
    }
}

public enum CommandKey
{
    Undefined,
    Ping,
    Pong,
    /// <summary>
    /// Join channel
    /// </summary>
    Join,
    /// <summary>
    /// Leave channel
    /// </summary>
    Part,
    /// <summary>
    /// Twitch Private Message
    /// </summary>
    Privmsg,
    // Twitch extensions
    /// <summary>
    /// Send message to a single user
    /// </summary>
    Whisper,
    /// <summary>
    /// Purge a user's messages
    /// </summary>
    Clearchat,
    /// <summary>
    /// Single message removal
    /// </summary>
    Clearmsg,
    /// <summary>
    /// Sent upon successful authentication (PASS/NICK command)
    /// </summary>
    GlobalUserState,
    /// <summary>
    /// Channel starts or stops host mode
    /// </summary>
    HostTarget,
    /// <summary>
    /// General notices from the server
    /// </summary>
    Notice,
    /// <summary>
    /// Rejoins channels after a restart
    /// </summary>
    Reconnect,
    /// <summary>
    /// Identifies the channel's chat settings
    /// </summary>
    RoomState,
    /// <summary>
    /// Announces Twitch-specific events to the channel
    /// </summary>
    UserNotice,
    /// <summary>
    /// Identifies a user's chat settings or properties
    /// </summary>
    UserState,
    /// <summary>
    /// Requesting an IRC capability
    /// </summary>
    Capability,
    // Numeric commands
    RplWelcome,
    RplYourHost,
    RplCreated,
    RplMyInfo,
    RplNamReply,
    RplEndOfNames,
    RplMotd,
    RplMotdStart,
    RplEndOfMotd,
}
