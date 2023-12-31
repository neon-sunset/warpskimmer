## Roadmap
- [x] API shape and prototype implementation
- [x] Port learnings and solve performance issues in `TwitchLib`(most popular C# Twitch library): done in https://github.com/TwitchLib/TwitchLib.Client/pull/230 and https://github.com/TwitchLib/TwitchLib.Client/pull/232
- [ ] Test coverage
- [ ] Documentation
- [ ] Productize the library and publish to nuget (blocked by U8String impl.)

## Disclaimer
This prototype depends on work-in-progress implementation of `U8String`.

In order to build and run the benchmarks, please make sure to
- Clone with  `git clone --recurse-submodules https://github.com/neon-sunset/warpskimmer`
- Install latest (left column) .NET 8 SDK from https://github.com/dotnet/installer#table
- `cd bench && dotnet run -c release`

## Why fast
- Significant improvements in struct optimizations in latest .NET versions
- Fully vectorized element scan operations on `{ReadOnly}Span<T>`s in CoreLib on all major ISAs
- Custom UTF-8 string primitive (`U8String`) which makes different treadeoffs than both `string` and `Utf8String` prototype that existed around .NET 5, leaning heavily towards in between Rust's and Golang's implementation choices:
  - Non-copying slicing and bounds-check and utf8-validation-free unsafe slicing API which preserves the original U8String type signature (can be boxed unlike `ROS<byte>`, can be used for lookup, stored in an array, etc. unlike `ROM<byte>`)
  - Being UTF-8 which improves the throughput of SIMD scanning by 2x
  - Being a `(byte[], int, int)` struct which allows the compiler to optimize away, forward and CSE operations on it where possible
- Not being compliant with full IRCv3 spec but rather targeting the exact message order and format of Twitch chat websocket stream
- Performing opportunistic tag separator matching with AdvSimd/SSE2 inside the parser loop avoiding `SpanHelpers.IndexOfAny...` call to vectorized search for the full span
- Simplicity of the implementation: by passing `ref U8String`, we can incrementally update the slice until we finish in a pointer math-like fashion without actually writing unsafe code (aside from ensuring that the offsets are correct to prevent malformed UTF-8 slices or dereferencing invalid memory ranges)
- Careful `ref U8String` dereferencing and ref updating to reduce write barriers yet enable JIT to reason about its state as a part of local scope
- Compact `Tag` layout enabled by `U8String.SplitPair` which holds interior string and split offsets - further reduces alloc size and write barriers on `Tag` assignment to `Tag[]`

## Perf Estimation
Time to parse 1000 sample lines from forsen chat stream.

Goal: <333ns (~1.36(6) GB/sec) per 884 byte line (worst case scenario from Forsen chat)
- [x] x86_64 (Zen 3: ~2.4 GB/sec)
- [x] arm64 (Apple M1 Pro: ~1.4GB/sec)

## X86_64
```txt
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.2134/22H2/2022Update/SunValley2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=8.0.100-rc.2.23425.8
  [Host]        : .NET 8.0.0 (8.0.23.42311), X64 RyuJIT AVX2
  DefaultJob    : .NET 8.0.0 (8.0.23.42311), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.0-rc.2.23423.11, X64 NativeAOT AVX2
```
| Method |       Runtime |  Count |            Mean |        Error |       StdDev |      Gen0 |  Allocated |
|------- |-------------- |------- |----------------:|-------------:|-------------:|----------:|-----------:|
|  Parse |      .NET 8.0 |      1 |        152.4 ns |      1.66 ns |      1.39 ns |    0.0367 |      616 B |
|  Parse | NativeAOT 8.0 |      1 |        183.4 ns |      0.79 ns |      0.70 ns |    0.0367 |      616 B |
|  Parse |      .NET 8.0 |   1000 |    161,045.4 ns |  2,010.62 ns |  1,880.73 ns |   36.3770 |   611368 B |
|  Parse | NativeAOT 8.0 |   1000 |    194,503.0 ns |    596.43 ns |    557.90 ns |   36.3770 |   611368 B |
|  Parse |      .NET 8.0 | 100000 | 16,844,497.7 ns | 72,071.46 ns | 67,415.69 ns | 3593.7500 | 60306100 B |
|  Parse | NativeAOT 8.0 | 100000 | 20,397,049.4 ns | 77,688.74 ns | 72,670.10 ns | 3593.7500 | 60306100 B |

## ARM64
```txt
BenchmarkDotNet=v0.13.5, OS=macOS 14.0 (23A5312d) [Darwin 23.0.0]
Apple M1 Pro, 1 CPU, 8 logical and 8 physical cores
.NET SDK=8.0.100-rc.2.23423.1
  [Host]        : .NET 8.0.0 (8.0.23.41814), Arm64 RyuJIT AdvSIMD
  DefaultJob    : .NET 8.0.0 (8.0.23.41814), Arm64 RyuJIT AdvSIMD
  NativeAOT 8.0 : .NET 8.0.0-rc.2.23418.14, Arm64 NativeAOT AdvSIMD
```
| Method |       Runtime |  Count |            Mean |         Error |        StdDev |      Gen0 |  Allocated |
|------- |-------------- |------- |----------------:|--------------:|--------------:|----------:|-----------:|
|  Parse |      .NET 8.0 |      1 |        285.5 ns |       1.40 ns |       1.31 ns |    0.0978 |      616 B |
|  Parse | NativeAOT 8.0 |      1 |        310.5 ns |       1.08 ns |       1.01 ns |    0.0978 |      616 B |
|  Parse |      .NET 8.0 |   1000 |    280,918.9 ns |   1,491.94 ns |   1,395.56 ns |   97.1680 |   611368 B |
|  Parse | NativeAOT 8.0 |   1000 |    308,498.1 ns |     608.62 ns |     475.17 ns |   97.1680 |   611368 B |
|  Parse |      .NET 8.0 | 100000 | 27,227,845.2 ns | 136,823.67 ns | 121,290.62 ns | 9593.7500 | 60306111 B |
|  Parse | NativeAOT 8.0 | 100000 | 29,900,051.0 ns | 117,515.19 ns | 109,923.78 ns | 9593.7500 | 60306100 B |