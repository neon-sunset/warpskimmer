## Perf Estimation
Time to parse 1000 sample lines from forsen chat stream.
Goal: <333ns per line
```txt
BenchmarkDotNet=v0.13.5, OS=macOS 14.0 [Darwin 23.0.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=8.0.100-preview.6.23307.25
  [Host]        : .NET 8.0.0 (8.0.23.30704), Arm64 RyuJIT AdvSIMD
  DefaultJob    : .NET 8.0.0 (8.0.23.30704), Arm64 RyuJIT AdvSIMD
  NativeAOT 8.0 : .NET 8.0.0-preview.6.23307.4, Arm64 NativeAOT AdvSIMD

| Method |       Runtime |     Mean |    Error |  StdDev |     Gen0 |  Allocated |
|------- |-------------- |---------:|---------:|--------:|---------:|-----------:|
|  Parse |      .NET 8.0 | 502.5 us |  2.40 us | 2.12 us | 198.2422 | 1218.75 KB |
|  Parse | NativeAOT 8.0 | 444.8 us |  1.56 us | 1.46 us | 198.7305 | 1218.75 KB |
```
