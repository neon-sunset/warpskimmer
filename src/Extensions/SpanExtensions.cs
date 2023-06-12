using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Feetlicker;

internal static class SpanExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<byte> FastSplit(
        this ReadOnlySpan<byte> span, byte value, out ReadOnlySpan<byte> rest)
    {
        var initial = span;

        if (span.Length >= Vector<byte>.Count)
        {
        Continue:
            ref var start = ref span.AsRef();

            var mask = Vector128.Create(value);
            var vector = Vector128.LoadUnsafe(ref start);
            var result = Vector128.Equals(vector, mask);
            if (result != Vector128<byte>.Zero)
            {
                // TODO: Optimize for AdvSimd with SHRN
                var index = BitOperations.LeadingZeroCount(result.ExtractMostSignificantBits());
                rest = Unsafe
                    .Add(ref start, index + 1)
                    .AsSpan(span.Length - index - 1);

                return start.AsSpan(index);
            }

            span = Unsafe
                .Add(ref start, Vector<byte>.Count)
                .AsSpan(span.Length - Vector<byte>.Count);

            if (span.Length >= Vector<byte>.Count)
            {
                goto Continue;
            }
        }

        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] == value)
            {
                rest = Unsafe
                    .Add(ref span.AsRef(), i + 1)
                    .AsSpan(span.Length - i - 1);
                return span.AsRef().AsSpan(i);
            }
        }

        rest = initial;
        return default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref T AsRef<T>(this ReadOnlySpan<T> span)
        where T : unmanaged
    {
        return ref MemoryMarshal.GetReference(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<T> AsSpan<T>(this ref T value, int length)
        where T : unmanaged
    {
        return MemoryMarshal.CreateReadOnlySpan(ref value, length);
    }
}
