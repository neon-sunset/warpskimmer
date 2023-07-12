using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using U8Primitives.Unsafe;

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
        var deref = source;
        if (deref[0] != (byte)'@')
        {
            return null;
        }

        // Last range is the remainder of the source
        var allTags = U8Marshal.Substring(deref, 1);
        var tagRanges = (stackalloc Range[128]);
        var tagCount = SplitTags(allTags, tagRanges);
        var tags = new Tag[tagCount];

        for (var i = 0; i < tags.Length; i++)
        {
            var tagValue = U8Marshal.Slice(allTags, tagRanges[i]);
            var separator = IndexOfSeparator(
                ref MemoryMarshal.GetReference<byte>(tagValue));

            tags[i] = separator is > 0 and <= 16
                ? new Tag(
                    U8Marshal.Slice(tagValue, ..separator),
                    U8Marshal.Slice(tagValue, (separator + 1)..))
                : Parse(tagValue);
        }

        source = U8Marshal.Slice(allTags, tagRanges[tagCount]);
        return tags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfSeparator(ref byte ptr)
    {
        var eqmask = Vector128.Equals(
            Vector128.LoadUnsafe(ref ptr),
            Vector128.Create((byte)'='));

        if (ArmBase.IsSupported)
        {
            var matches = AdvSimd
                .ShiftRightLogicalNarrowingLower(eqmask.AsUInt16(), 4)
                .AsUInt64()
                .ToScalar();
            return BitOperations.TrailingZeroCount(matches) >> 2;
        }
        else
        {
            return BitOperations.TrailingZeroCount(
                eqmask.ExtractMostSignificantBits());
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
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