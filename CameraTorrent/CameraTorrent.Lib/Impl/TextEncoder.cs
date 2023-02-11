using System.IO;
using Base45Utility;
using CameraTorrent.Lib.API;

namespace CameraTorrent.Lib.Impl
{
    public sealed class TextEncoder : IEncoder
    {
        private readonly Base45 _tool = new();

        public string Encode(Stream input)
        {
            var bytes = ((MemoryStream)input).ToArray();
            var text = _tool.Encode(bytes);
            return text;
        }

        public Stream Decode(string input)
        {
            var bytes = _tool.Decode(input);
            var stream = new MemoryStream(bytes);
            return stream;
        }
    }
}