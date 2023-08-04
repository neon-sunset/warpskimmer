## Roadmap
- [x] API shape and prototype implementation
- [x] Port learnings and solve performance issues in `TwitchLib`(most popular C# Twitch library): done in https://github.com/TwitchLib/TwitchLib.Client/pull/230 and https://github.com/TwitchLib/TwitchLib.Client/pull/232
- [ ] Test coverage
- [ ] Documentation
- [ ] Productize the library and publish to nuget (blocked by U8String impl.)

## Disclaimer
This prototype depends on work-in-progress implementation of `U8String` at https://github.com/neon-sunset/U8String/tree/enumerators-and-canonical-ops (specifically this branch) which is required for building the library and running the benchmarks.

Why is this fast:
- Significant improvements in struct optimizations in latest .NET versions
- Fully vectorized element scan operations on `{ReadOnly}Span<T>`s in CoreLib on all major ISAs
- Custom UTF-8 string primitive (`U8String`) which makes different treadeoffs than both `string` and `Utf8String` prototype that existed around .NET 5, leaning heavily towards in between Rust's and Golang's implementation choices:
  - Non-copying slicing and bounds-check and utf8-validation-free unsafe slicing API which preserves the original U8String type signature (can be boxed unlike `ROS<byte>`, can be used for lookup, stored in an array, etc. unlike `ROM<byte>`)
  - `split_once()`-like `Split{First/Last}` methods which return compact "split pair" struct used by `Tag`s
  - Being UTF-8 which improves the throughput of SIMD scanning by 2x
  - Being a `(byte[], int, int)` struct which allows the compiler to optimize away, forward and CSE operations on it where possible
- Not being compliant with full IRCv3 spec but rather targeting the exact message order and format of Twitch chat websocket stream
- Performing opportunistic tag separator matching with AdvSimd/SSE2 inside the parser loop avoiding `SpanHelpers.IndexOfAny...` call to vectorized search for the full span
- Simplicity of the implementation: by passing `ref U8String`, we can incrementally update the slice until we finish in a pointer math-like fashion without actually writing unsafe code (aside from ensuring that the offsets are correct to prevent malformed UTF-8 slices or dereferencing invalid memory ranges)

## Perf Estimation
Time to parse 1000 sample lines from forsen chat stream.

Goal: <333ns (2.529 GB/sec) per 884 byte line (worst case scenario from Forsen chat)
- [x] x86_64 (Zen 3: 4.015 GB/sec)
- [ ] arm64 (Apple M1 Pro: pending)

```txt
BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1992/22H2/2022Update/SunValley2)
AMD Ryzen 7 5800X, 1 CPU, 16 logical and 8 physical cores
.NET SDK=8.0.100-rc.1.23403.1
  [Host]        : .NET 8.0.0 (8.0.23.38103), X64 RyuJIT AVX2
  DefaultJob    : .NET 8.0.0 (8.0.23.38103), X64 RyuJIT AVX2
  NativeAOT 8.0 : .NET 8.0.0-rc.1.23381.3, X64 NativeAOT AVX2
``````

| Method |           Job |       Runtime |  Count |            Mean |         Error |        StdDev |      Gen0 |  Allocated |
|------- |-------------- |-------------- |------- |----------------:|--------------:|--------------:|----------:|-----------:|
|  Parse |    DefaultJob |      .NET 8.0 |      1 |        208.3 ns |       4.14 ns |       8.17 ns |    0.0367 |      616 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 |      1 |        239.2 ns |       1.62 ns |       1.44 ns |    0.0367 |      616 B |
|  Parse |    DefaultJob |      .NET 8.0 |   1000 |    205,736.1 ns |   1,937.34 ns |   1,717.40 ns |   36.3770 |   611368 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 |   1000 |    248,170.9 ns |   1,855.09 ns |   1,735.26 ns |   36.1328 |   611368 B |
|  Parse |    DefaultJob |      .NET 8.0 | 100000 | 20,992,206.5 ns | 135,146.84 ns | 112,853.73 ns | 3593.7500 | 60306100 B |
|  Parse | NativeAOT 8.0 | NativeAOT 8.0 | 100000 | 25,254,594.6 ns | 100,991.60 ns |  94,467.60 ns | 3593.7500 | 60306100 B |
