using System.Buffers;
using System.Runtime.CompilerServices;

namespace Feetlicker;

public readonly record struct Tag
{
    private static readonly SearchValues<byte> Delimiters = SearchValues
        .Create(stackalloc byte[] { (byte)' ', (byte)';' });

    // TODO: Use enum for key, which is slower but also more type-safe
    public U8String Key { get; }

    public U8String Value { get; }

    public Tag(U8String key, U8String value)
    {
        Key = key;
        Value = value;
    }

    public static Tag[]? ParseAll(ref U8String source)
    {
        if (source is not [(byte)'@', ..var tagsValue])
        {
            return null;
        }

        // Last range is the remainder of the source
        var tagRanges = (stackalloc Range[128]);
        var tagCount = SplitTags(tagsValue, tagRanges);
        var tags = new Tag[tagCount];

        for (var i = 0; i < tags.Length; i++)
        {
            tags[i] = Parse(tagsValue[tagRanges[i]]);
        }

        source = tagsValue[tagRanges[tagCount]];
        return tags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tag Parse(U8String value)
    {
        var (key, tagValue) = value.SplitFirst((byte)'=');

        return new Tag(key, tagValue);
    }

    private static int SplitTags(ReadOnlySpan<byte> src, Span<Range> ranges)
    {
        var rangeCount = 0;
        var sourceIndex = 0;

        for (var matchCount = 0; matchCount < ranges.Length; matchCount++)
        {
            var separatorIndex = src[sourceIndex..].IndexOfAny(Delimiters);
            var delimiterOffset = sourceIndex + separatorIndex;
            if (src[delimiterOffset] is (byte)' ')
            {
                ranges[matchCount] = sourceIndex..delimiterOffset;
                ranges[matchCount + 1] = ++delimiterOffset..src.Length;
                rangeCount++;
                break;
            }

            ranges[matchCount] = sourceIndex..delimiterOffset;
            sourceIndex += separatorIndex + 1;
            rangeCount++;
        }

        return rangeCount;
    }
}

public enum TagKey
{
    Undefined
}