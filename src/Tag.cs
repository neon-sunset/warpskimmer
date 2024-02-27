using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

using U8.InteropServices;
using U8.Primitives;

namespace Warpskimmer;

public readonly record struct Tag
{
    private readonly U8SplitPair TagSplit;

    public U8String Key => TagSplit.Segment;
    public U8String Value => TagSplit.Remainder;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Tag(U8SplitPair tagSplit)
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
            .SliceUnsafe(deref, 1)
            .SplitFirst((byte)' ');

        var split = deref.Split(";"u8);
        var tags = new Tag[(uint)split.Count];
        var separator = Vector128.Create((byte)'=');
        var i = 0;

        ref var dst = ref tags.AsSpan().AsRef();
        foreach (var tagValue in split)
        {
            var splitOffset = IndexOfSeparator(
                in U8Marshal.GetReference(tagValue), separator);

            dst.Add(i++) = splitOffset < 16
                ? new(U8Marshal.CreateSplitPairUnsafe(tagValue, splitOffset, 1))
                : Parse(tagValue);
        }

        return tags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfSeparator(ref readonly byte ptr, Vector128<byte> separator)
    {
        var eqmask = Vector128.Equals(Vector128.LoadUnsafe(in ptr), separator);

        if (AdvSimd.IsSupported)
        {
            var matches = AdvSimd
                .ShiftRightLogicalNarrowingLower(eqmask.AsUInt16(), 4)
                .AsUInt64()
                .ToScalar();
            return BitOperations.TrailingZeroCount(matches) >> 2;
        }

        return BitOperations.TrailingZeroCount(eqmask.ExtractMostSignificantBits());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tag Parse(U8String value)
    {
        return new Tag(value.SplitFirst((byte)'='));
    }
}
