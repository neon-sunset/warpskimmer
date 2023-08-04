using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Warpskimmer;

static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ReadOnlySpan<T> SliceUnsafe<T>(this ReadOnlySpan<T> span, int index)
    {
        return MemoryMarshal.CreateReadOnlySpan(
            ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)index),
            span.Length - index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref T IndexUnsafe<T>(this Span<T> span, int index)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)index);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref readonly T IndexUnsafe<T>(this ReadOnlySpan<T> span, int index)
    {
        return ref Unsafe.Add(ref MemoryMarshal.GetReference(span), (nint)(uint)index);
    }
}
