using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Warpskimmer;

static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref T AsRef<T>(this Span<T> source)
        where T : struct
    {
        return ref MemoryMarshal.GetReference(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ref T Add<T>(this ref T ptr, int offset)
        where T : struct
    {
        return ref Unsafe.Add(ref ptr, (nint)(uint)offset);
    }
}
