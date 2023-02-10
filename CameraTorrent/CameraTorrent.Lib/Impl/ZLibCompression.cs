using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;

namespace CameraTorrent.Lib.Impl
{
    public sealed class ZLibCompression : ICompression
    {
        public CompressionLevel Level { get; set; } = CompressionLevel.Optimal;

        public async Task<Stream> Decompress(Stream input)
        {
            var output = new MemoryStream();
            await using var zip = new ZLibStream(input, CompressionMode.Decompress);
            await zip.CopyToAsync(output);
            zip.Close();
            output.Position = 0L;
            return output;
        }

        public async Task<Stream> Compress(Stream input)
        {
            var output = new MemoryStream();
            await using var zip = new ZLibStream(output, Level, true);
            await input.CopyToAsync(zip);
            zip.Close();
            output.Position = 0L;
            return output;
        }
    }
}