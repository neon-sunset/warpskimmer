// Proposal: https://github.com/dotnet/runtime/issues/75317
// This code is licensed under MIT license
// (c) 2022 neon-sunset

using System.Runtime.CompilerServices;

namespace System;

public static class SplitExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (string Left, string Right) SplitFirst(this string source, char separator)
    {
        var (left, right) = source
            .AsSpan()
            .SplitFirst(separator);

        return (left.ToString(), right.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitPair<T> SplitFirst<T>(this Span<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.IndexOf(separator);

        return separatorIndex > -1
            ? new(source[..separatorIndex], source[(separatorIndex + 1)..])
            : new(source, Span<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySplitPair<T> SplitFirst<T>(this ReadOnlySpan<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.IndexOf(separator);

        return separatorIndex > -1
            ? new(source[..separatorIndex], source[(separatorIndex + 1)..])
            : new(source, ReadOnlySpan<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Memory<T> Left, Memory<T> Right) SplitFirst<T>(this Memory<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.Span.IndexOf(separator);

        return separatorIndex > -1
            ? (source[..separatorIndex], source[(separatorIndex + 1)..])
            : (source, Memory<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (ReadOnlyMemory<T> Left, ReadOnlyMemory<T> Right) SplitFirst<T>(this ReadOnlyMemory<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.Span.IndexOf(separator);

        return separatorIndex > -1
            ? (source[..separatorIndex], source[(separatorIndex + 1)..])
            : (source, ReadOnlyMemory<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (string Left, string Right) SplitLast(this string source, char separator)
    {
        var (left, right) = source
            .AsSpan()
            .SplitLast(separator);

        return (left.ToString(), right.ToString());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitPair<T> SplitLast<T>(this Span<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.LastIndexOf(separator);

        return separatorIndex > -1
            ? new(source[..separatorIndex], source[(separatorIndex + 1)..])
            : new(source, Span<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySplitPair<T> SplitLast<T>(this ReadOnlySpan<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.LastIndexOf(separator);

        return separatorIndex > -1
            ? new(source[..separatorIndex], source[(separatorIndex + 1)..])
            : new(source, ReadOnlySpan<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (Memory<T> Left, Memory<T> Right) SplitLast<T>(this Memory<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.Span.LastIndexOf(separator);

        return separatorIndex > -1
            ? (source[..separatorIndex], source[(separatorIndex + 1)..])
            : (source, Memory<T>.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static (ReadOnlyMemory<T> Left, ReadOnlyMemory<T> Right) SplitLast<T>(this ReadOnlyMemory<T> source, T separator)
        where T : IEquatable<T>
    {
        var separatorIndex = source.Span.LastIndexOf(separator);

        return separatorIndex > -1
            ? (source[..separatorIndex], source[(separatorIndex + 1)..])
            : (source, ReadOnlyMemory<T>.Empty);
    }

    public readonly ref struct SplitPair<T>
    {
        public readonly Span<T> Left;

        public readonly Span<T> Right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SplitPair(Span<T> left, Span<T> right)
        {
            Left = left;
            Right = right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out Span<T> left, out Span<T> right)
        {
            left = Left;
            right = Right;
        }
    }

    public readonly ref struct ReadOnlySplitPair<T>
    {
        public readonly ReadOnlySpan<T> Left;

        public readonly ReadOnlySpan<T> Right;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySplitPair(ReadOnlySpan<T> left, ReadOnlySpan<T> right)
        {
            Left = left;
            Right = right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out ReadOnlySpan<T> left, out ReadOnlySpan<T> right)
        {
            left = Left;
            right = Right;
        }
    }
}
