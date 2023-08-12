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

        (deref, source) = U8Marshal
            .Slice(deref, 1)
            .SplitFirst((byte)' ');

        var split = deref.Split((byte)';');
        var tags = new Tag[split.Count];
        var tagsSpan = tags.AsSpan();
        var i = 0;

        foreach (var tagValue in split)
        {
            var splitOffset = IndexOfSeparator(
                ref Unsafe.AsRef(U8Marshal.GetReference(tagValue)));

            tagsSpan.IndexUnsafe(i++) = splitOffset < 16
                ? new(U8Marshal.CreateSplitPair(tagValue, splitOffset, 1))
                : Parse(tagValue);
        }

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
}
