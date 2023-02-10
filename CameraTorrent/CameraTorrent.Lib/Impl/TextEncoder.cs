using System;
using System.IO;
using CameraTorrent.Lib.API;
using Roydl.Text.BinaryToText;

namespace CameraTorrent.Lib.Impl
{
    public sealed class TextEncoder : IEncoder
    {
        public BinToTextEncoding Level { get; set; } = BinToTextEncoding.Base91;

        public string Encode(Stream input)
        {
            var bytes = ((MemoryStream)input).ToArray();
            var text = Level == BinToTextEncoding.Base64
                ? Convert.ToBase64String(bytes)
                : bytes.Encode(Level);
            return text;
        }

        public Stream Decode(string input)
        {
            var bytes = Level == BinToTextEncoding.Base64
                ? Convert.FromBase64String(input)
                : input.Decode(Level);
            var stream = new MemoryStream(bytes);
            return stream;
        }
    }
}