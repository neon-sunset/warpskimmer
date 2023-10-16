namespace Warpskimmer.Benchmark;

public class ParseMessage
{
    private U8String[] Lines = null!;

    public int Count;

    public void Setup()
    {
        using var file = File.OpenHandle("data.txt");

        Lines = U8String
            .Read(file)
            .Lines
            .Take(Count)
            .ToArray();
    }

    public void Parse()
    {
        foreach (var line in Lines)
        {
            _ = Message.Parse(line);
        }
    }
}
