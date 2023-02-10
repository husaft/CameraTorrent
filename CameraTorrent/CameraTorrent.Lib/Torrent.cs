using System.IO;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using CameraTorrent.Lib.Impl;

namespace CameraTorrent.Lib
{
    public sealed class Torrent
    {
        public ICompression Compression { get; set; } = new ZLibCompression();
        public IEncoder Encoder { get; set; } = new TextEncoder();
        public IBarcoder Barcoder { get; set; } = new QRBarcoder();

        public async Task<Stream> Unpack(Stream input)
        {
            var barcode = await Barcoder.Read(input);
            var contents = RemoveHeader(barcode);
            var decoded = Encoder.Decode(contents);
            var decompressed = await Compression.Decompress(decoded);
            return decompressed;
        }

        public async Task<Stream> Pack(Stream input)
        {
            var compressed = await Compression.Compress(input);
            var encoded = Encoder.Encode(compressed);
            var contents = AddHeader(encoded);
            var barcode = await Barcoder.Write(contents);
            return barcode;
        }

        private const string Magic = "CT1:";

        private static string AddHeader(string data)
        {
            return $"{Magic}{data}";
        }

        private static string RemoveHeader(string data)
        {
            return data[Magic.Length..];
        }
    }
}