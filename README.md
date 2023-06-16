## Perf Estimation
```txt
BenchmarkDotNet=v0.13.5, OS=macOS 14.0 [Darwin 23.0.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=8.0.100-preview.6.23307.25
  [Host]        : .NET 8.0.0 (8.0.23.30704), Arm64 RyuJIT AdvSIMD
  DefaultJob    : .NET 8.0.0 (8.0.23.30704), Arm64 RyuJIT AdvSIMD
  NativeAOT 8.0 : .NET 8.0.0-preview.6.23307.4, Arm64 NativeAOT AdvSIMD


| Method |       Runtime |     Mean |    Error |  StdDev |     Gen0 |  Allocated |
|------- |-------------- |---------:|---------:|--------:|---------:|-----------:|
|  Parse |      .NET 8.0 | 544.7 us |  8.89 us | 7.42 us | 163.0859 | 1000.27 KB |
|  Parse | NativeAOT 8.0 | 519.5 us | 10.01 us | 8.87 us | 163.0859 | 1000.27 KB |
```