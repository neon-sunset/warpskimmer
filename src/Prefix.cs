namespace Feetlicker;

public readonly record struct Prefix(
    U8String Host,
    U8String? Nick = null,
    U8String? User = null)
{
    public static unsafe Prefix? Parse(ref ReadOnlySpan<byte> source)
    {
        throw new NotImplementedException();
    }
}