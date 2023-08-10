using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using U8Primitives.InteropServices;

namespace Warpskimmer;

public readonly record struct Tag
{
    private static readonly SearchValues<byte> Delimiters = SearchValues.Create(" ;"u8);

    private readonly SplitPair TagSplit;

    public U8String Key => TagSplit.Segment;
    public U8String Value => TagSplit.Remainder;

    public Tag(SplitPair tagSplit)
    {
        TagSplit = tagSplit;
    }

    public static Tag[]? ParseAll(ref U8String source)
    {
        var deref = source;
        if (U8Marshal.GetReference(deref) != (byte)'@')
        {
            return null;
        }

        // Last range is the remainder of the source
        var allTags = U8Marshal.Slice(deref, 1);
        var tagRanges = (stackalloc Range[128]);
        var tagCount = SplitTags(allTags, tagRanges);
        var tags = new Tag[tagCount];
        var tagSpan = tags.AsSpan();

        for (var i = 0; i < tagSpan.Length; i++)
        {
            var tagValue = U8Marshal.Slice(allTags, tagRanges.IndexUnsafe(i));
            var splitOffset = IndexOfSeparator(
                ref MemoryMarshal.GetReference(U8Marshal.GetSpan(tagValue)));

            tagSpan.IndexUnsafe(i) = splitOffset < 16
                ? new(U8Marshal.CreateSplitPair(tagValue, splitOffset, 1))
                : Parse(tagValue);
        }

        source = U8Marshal.Slice(allTags, tagRanges.IndexUnsafe(tagCount));
        return tags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfSeparator(ref byte ptr)
    {
        var eqmask = Vector128.Equals(
            Vector128.LoadUnsafe(ref ptr),
            Vector128.Create((byte)'='));

        if (AdvSimd.IsSupported)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tag Parse(U8String value)
    {
        return new Tag(value.SplitFirst((byte)'='));
    }

    private static int SplitTags(ReadOnlySpan<byte> src, Span<Range> ranges)
    {
        var rangeCount = 0;
        var sourceIndex = 0;

        for (var matchCount = 0; matchCount < ranges.Length; matchCount++)
        {
            var separatorIndex = src
                .SliceUnsafe(sourceIndex)
                .IndexOfAny(Delimiters);
            var delimiterOffset = sourceIndex + separatorIndex;
            if (src.IndexUnsafe(delimiterOffset) is (byte)' ')
            {
                ranges.IndexUnsafe(matchCount) = sourceIndex..delimiterOffset;
                ranges.IndexUnsafe(matchCount + 1) = ++delimiterOffset..src.Length;
                rangeCount++;
                break;
            }

            ranges.IndexUnsafe(matchCount) = sourceIndex..delimiterOffset;
            sourceIndex += separatorIndex + 1;
            rangeCount++;
        }

        return rangeCount;
    }
}
