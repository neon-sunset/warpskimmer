# TODO
- [x] ~~Handle 'CAP REQ :twitch.tv/membership' and 'CAP REQ :twitch.tv/tags'~~ Seems no other impl. handles this so I guess it's okay?
- [ ] ~~Refactor Range-based tokenization into pure int offset-based~~ Not needed for now until tag parsing is rewritten into a better form (e.g. count tags -> broadcast parent U8String slice byte[] reference onto tag array, then vector match -> conditional move offsets onto tag array (if that's even expressible in AdvSimd/SSE4/AVX2))
- [ ] Escaping (impl. a separate escaper utility and/or an extension on top of Tags, this cannot not allocate sadly)
- [x] Message parsing
- [x] Prefix parsing
- [x] Command parsing
- [x] Tag parsing
- [x] ~~Stateful bytestream reader/parser (Pipes?)~~ Given this will use U8String once released, the consumers can simply use u8str.Lines.Select(Message.Parse).ToArray() and it will be reasonably fast
- [ ] Consider writing a custom message iterator on top of U8String
