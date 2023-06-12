namespace Feetlicker;

public readonly record struct Tag
{
    public TagKey Key { get; }

    public U8String Value { get; }

    public Tag(TagKey key, U8String value)
    {
        Key = key;
        Value = value;
    }

    public static Tag Parse(ReadOnlySpan<byte> value, out int bytesRead)
    {
        throw new NotImplementedException();
    }

    public static unsafe Tag[]? ParseAll(ref ReadOnlySpan<byte> source)
    {
        throw new NotImplementedException();
    }
}

public enum TagKey
{
    Undefined
}