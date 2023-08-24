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
- [ ] arm64 (Apple M1 Pro: pending)

```txt
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.2134/22H2/2022Update/SunValley2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=8.0.100-rc.2.23417.14
  [Host]        : .NET 8.0.0 (8.0.23.41404), X64 RyuJIT AVX2
  DefaultJob    : .NET 8.0.0 (8.0.23.41404), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.0-rc.1.23414.4, X64 NativeAOT AVX2
``````

| Method |           Job |       Runtime |  Count |            Mean |         Error |        StdDev |      Gen0 |  Allocated |
|------- |-------------- |-------------- |------- |----------------:|--------------:|--------------:|----------:|-----------:|
|  Parse |    DefaultJob |      .NET 8.0 |      1 |        155.6 ns |       1.39 ns |       1.30 ns |    0.0367 |      616 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 |      1 |        190.0 ns |       1.78 ns |       1.58 ns |    0.0367 |      616 B |
|  Parse |    DefaultJob |      .NET 8.0 |   1000 |    168,525.8 ns |   2,078.27 ns |   1,735.45 ns |   36.3770 |   611368 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 |   1000 |    201,164.3 ns |   1,281.51 ns |   1,198.72 ns |   36.3770 |   611368 B |
|  Parse |    DefaultJob |      .NET 8.0 | 100000 | 16,494,268.3 ns | 122,753.73 ns | 108,817.98 ns | 3593.7500 | 60306100 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 | 100000 | 20,883,511.5 ns |  65,064.85 ns |  60,861.70 ns | 3593.7500 | 60306100 B |
