using System.Runtime.CompilerServices;

namespace Feetlicker;

public readonly record struct Tag
{
    // TODO: Use enum for key, which is slower but also more type-safe
    public U8String Key { get; }

    public U8String Value { get; }

    public Tag(U8String key, U8String value)
    {
        Key = key;
        Value = value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tag Parse(U8String value)
    {
        var (key, tagValue) = value.SplitFirst((byte)'=');

        return new Tag(key, tagValue);
    }

    public static Range[] TokenizeAll(ReadOnlySpan<byte> value, int offset)
    {
        var ranges = (stackalloc Range[128]);
        var tagCount = value.Split(ranges, (byte)';');

        var result = new Range[tagCount];
        for (var i = 0; i < tagCount; i++)
        {
            var range = ranges[i];
            result[i] = (range.Start.Value + offset)..(range.End.Value + offset);
        }

        return result;
    }
}

public enum TagKey
{
    Undefined
}