using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;

using U8Primitives.InteropServices;

namespace Warpskimmer;

public readonly record struct Tag
{
    readonly U8Source _source;
    readonly U8Range _key;
    readonly U8Range _value;

    public U8String Key => U8Marshal.Slice(_source, _key);
    public U8String Value => U8Marshal.Slice(_source, _value);

    public Tag(U8SplitPair tagSplit)
    {
        _source = tagSplit.Segment.Source;
        _key = tagSplit.Segment.Range;
        _value = tagSplit.Remainder.Range;
    }

    internal Tag(U8Source source, U8Range key, U8Range value)
    {
        _source = source;
        _key = key;
        _value = value;
    }

    public static Tag Parse(U8String value)
    {
        return new Tag(value.SplitFirst((byte)'='));
    }
}

public class TagList : IList<Tag>
{
    readonly U8Source Source;
    readonly U8Range Range;
    readonly (U8Range, U8Range)[] Tags;

    internal TagList(U8String raw, (U8Range, U8Range)[] tags)
    {
        Source = raw.Source;
        Range = raw.Range;
        Tags = tags;
    }

    public U8String Raw => U8Marshal.Slice(Source, Range);

    public Tag this[int index]
    {
        get
        {
            var (key, value) = Tags[index];
            return new(Source, key, value);
        }
        set => throw new NotSupportedException();
    }

    public int Count => Tags.Length;

    internal static TagList? Parse(ref U8String source)
    {
        var deref = source;
        if (U8Marshal.GetReference(deref) is (byte)'@')
        {
            (deref, source) = U8Marshal
                .Slice(deref, 1)
                .SplitFirst((byte)' ');

            var count = deref.AsSpan().Count((byte)';') + 1;
            var tags = new (U8Range, U8Range)[count];
            var tagsSpan = tags.AsSpan();

            var i = 0;
            var eqsign = Vector128.Create((byte)'=');
            foreach (var tagValue in deref.Split(";"u8))
            {
                var splitOffset = IndexOf(in U8Marshal.GetReference(tagValue), eqsign);

                splitOffset = splitOffset < 16
                    ? splitOffset
                    : tagValue.IndexOf((byte)'=');

                var key = U8Range.Slice(tagValue, 0, splitOffset);
                var value = U8Range.Slice(tagValue, splitOffset + 1);

                tagsSpan.IndexUnsafe(i++) = (key, value);
            }

            return new(deref, tags);
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int IndexOf(ref readonly byte ptr, Vector128<byte> separator)
    {
        var value = Vector128.LoadUnsafe(in ptr);
        var eqmask = Vector128.Equals(value, separator);

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

    bool ICollection<Tag>.IsReadOnly => true;

    bool ICollection<Tag>.Contains(Tag item)
    {
        foreach (var tag in this)
        {
            if (tag.Equals(item))
            {
                return true;
            }
        }

        return false;
    }

    void ICollection<Tag>.CopyTo(Tag[] array, int arrayIndex)
    {
        var source = Source;
        var tags = Tags;
        var destination = array.AsSpan()[arrayIndex..][..tags.Length];

        for (var i = 0; i < tags.Length; i++)
        {
            var (key, value) = tags[i];
            destination[i] = new(source, key, value);
        }
    }

    int IList<Tag>.IndexOf(Tag item)
    {
        var index = 0;
        foreach (var tag in this)
        {
            if (tag.Key.Equals(item))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    public Enumerator GetEnumerator() => new(Source, Tags);

    IEnumerator<Tag> IEnumerable<Tag>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<Tag>
    {
        readonly U8Source Source;
        readonly (U8Range, U8Range)[] Tags;
        int Index;

        internal Enumerator(U8Source source, (U8Range, U8Range)[] tags)
        {
            Source = source;
            Tags = tags;
            Index = -1;
        }

        public readonly Tag Current
        {
            get
            {
                var (key, value) = Tags[Index];
                return new(Source, key, value);
            }
        }

        readonly object IEnumerator.Current => Current;

        public bool MoveNext() => ++Index < Tags.Length;

        public void Reset() => Index = -1;

        public readonly void Dispose() { }
    }

    void IList<Tag>.Insert(int index, Tag item) => throw new NotSupportedException();
    void IList<Tag>.RemoveAt(int index) => throw new NotSupportedException();
    void ICollection<Tag>.Add(Tag item) => throw new NotSupportedException();
    void ICollection<Tag>.Clear() => throw new NotSupportedException();
    bool ICollection<Tag>.Remove(Tag item) => throw new NotSupportedException();
}
