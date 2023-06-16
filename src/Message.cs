using System.Runtime.CompilerServices;

namespace Feetlicker;

public record Message(
    Tag[]? Tags,
    Prefix? Prefix,
    Command Command,
    U8String? Channel,
    U8String? Parameters)
{
    private record TokenizedView(
        Range[]? Tags,
        Range? Prefix,
        Range Command,
        Range? Channel,
        Range? Parameters);

    // Ideally up to 333ns per line but that would probably be too hard
    public static Message Parse(U8String line)
    {
        Tag[]? tags = null;
        Prefix? prefix = null;
        U8String? channel = null;
        U8String? parameters = null;

        // Calculate exact offsets/ranges of message segments.
        // This design enables us to find all separators in a single pass,
        // which is friendly to CPU design and allows for future optimizations
        // since slicing is almost free and tokenization + segment parsing dominate the execution time.
        var tokenizedView = Tokenize(line);

        var tagRanges = tokenizedView.Tags;
        if (tagRanges != null)
        {
            tags = new Tag[tagRanges.Length];
            for (var i = 0; i < tagRanges.Length; i++)
            {
                var tagString = line[tagRanges[i]];
                tags[i] = Tag.Parse(tagString);
            }
        }

        if (tokenizedView.Prefix is Range prefixRange)
        {
            prefixRange = (prefixRange.Start.Value + 1)..prefixRange.End; // Remove ':' prefix
            prefix = Feetlicker.Prefix.Parse(line[prefixRange]);
        }

        var command = Command.Parse(line.AsSpan(tokenizedView.Command));

        if (tokenizedView.Channel is Range channelRange)
        {
            channelRange = (channelRange.Start.Value + 1)..channelRange.End; // Remove '#' prefix
            channel = line[channelRange];
        }

        if (tokenizedView.Parameters is Range paramsRange)
        {
            // Remove ':' prefix
            if (line[paramsRange][0] is (byte)':')
            {
                paramsRange = (paramsRange.Start.Value + 1)..paramsRange.End;
            }
            parameters = line[paramsRange];
        }

        return new Message(tags, prefix, command, channel, parameters);
    }

    private static TokenizedView Tokenize(ReadOnlySpan<byte> line)
    {
        // TODO: Implement single-pass multi-token matching
        // For now we will just recursively
        // -> look for segment separators
        // -> look for sub-segment (tag) separators
        // -> look for sub-sub-segment (tag key/value) separators
        Range[]? tags = null;
        Range? prefix = null;
        Range? channel = null;
        Range? parameters = null;

        var tagsEnd = 0;
        if (line[0] is (byte)'@')
        {
            var tagsLength = line.IndexOf((byte)' ');
            tagsEnd = tagsLength + 1;
            tags = Tag.TokenizeAll(line[1..tagsLength], 1);
        }

        // 0: prefix, 1: command, 2: channel, 3: parameters
        var ranges = new Range[4].AsSpan();
        var rangeCount = line[tagsEnd..].Split(ranges, (byte)' ');
        ranges = ranges[..rangeCount];
        for (var i = 0; i < ranges.Length; i++)
        {
            var range = ranges[i];
            ranges[i] = (range.Start.Value + tagsEnd)..(range.End.Value + tagsEnd);
        }

        if (First(line, ranges) is (byte)':')
        {
            prefix = ranges[0];
            ranges = ranges[1..];
        }

        var command = ranges[0];
        ranges = ranges[1..];

        if (!ranges.IsEmpty && First(line, ranges) is (byte)'#')
        {
            channel = ranges[0];
            ranges = ranges[1..];
        }

        if (!ranges.IsEmpty)
        {
            parameters = ranges[0].Start..line.Length;
        }

        return new TokenizedView(tags, prefix, command, channel, parameters);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte First(ReadOnlySpan<byte> line, Span<Range> segments) => line[segments[0]][0];
}
