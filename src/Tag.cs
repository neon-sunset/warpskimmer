using System.Buffers;
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

    public static List<Tag>? ParseAll(ref U8String source)
    {
        if (source is not [(byte)'@', ..var tagsValue])
        {
            return null;
        }

        (tagsValue, source) = tagsValue.SplitFirst((byte)' ');

        var tags = new List<Tag>(32);
        do
        {
            (var tag, tagsValue) = tagsValue.SplitFirst((byte)';');
            tags.Add(Parse(tag));
        } while (tagsValue.Length > 0);

        return tags;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Tag Parse(U8String value)
    {
        var (key, tagValue) = value.SplitFirst((byte)'=');

        return new Tag(key, tagValue);
    }
}

public enum TagKey
{
    Undefined
}