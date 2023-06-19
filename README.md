## Perf Estimation
Time to parse 1000 sample lines from forsen chat stream.
Goal: <333ns per line
```txt
BenchmarkDotNet=v0.13.5, OS=macOS 14.0 [Darwin 23.0.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=8.0.100-preview.6.23316.13
  [Host]        : .NET 8.0.0 (8.0.23.31507), Arm64 RyuJIT AdvSIMD
  DefaultJob    : .NET 8.0.0 (8.0.23.31507), Arm64 RyuJIT AdvSIMD
  NativeAOT 8.0 : .NET 8.0.0-preview.6.23315.7, Arm64 NativeAOT AdvSIMD


| Method |           Job |       Runtime |  Count |            Mean |         Error |        StdDev |       Gen0 |  Allocated |
|------- |-------------- |-------------- |------- |----------------:|--------------:|--------------:|-----------:|-----------:|
|  Parse |    DefaultJob |      .NET 8.0 |      1 |        421.6 ns |       0.46 ns |       0.43 ns |     0.1173 |      736 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 |      1 |        407.2 ns |       0.48 ns |       0.45 ns |     0.1173 |      736 B |
|  Parse |    DefaultJob |      .NET 8.0 |   1000 |    451,651.2 ns |     330.19 ns |     275.72 ns |   116.2109 |   729824 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 |   1000 |    419,854.8 ns |     456.60 ns |     427.10 ns |   116.2109 |   729824 B |
|  Parse |    DefaultJob |      .NET 8.0 | 100000 | 45,966,486.9 ns |  90,863.99 ns |  84,994.24 ns | 11454.5455 | 71874851 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 | 100000 | 42,378,222.0 ns | 278,878.22 ns | 260,862.86 ns | 11416.6667 | 71874816 B |
```
