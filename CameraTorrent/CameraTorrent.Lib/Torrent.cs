using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using CameraTorrent.Lib.Impl;
using CameraTorrent.Lib.Model;
using CameraTorrent.Lib.Util;

namespace CameraTorrent.Lib
{
    public sealed class Torrent
    {
        public ICompression Compression { get; set; } = new ZLibCompression();
        public IEncoder Encoder { get; set; } = new TextEncoder();
        public IBarcoder Barcoder { get; set; } = new QRBarcoder();
        public int MaxSize { get; set; } = 64 * 1024 * 1024;
        public int PkgSize { get; set; } = 2700;

        public async Task<bool> Unpack(Stream input, Bucket bucket)
        {
            var barcode = await Barcoder.Read(input);
            var got = Packets.Parse(barcode);
            if (got == null)
            {
                return false;
            }
            if (got.Value.Item1 is { } data)
            {
                bucket.Import(data);
                return true;
            }
            if (got.Value.Item2 is { } meta)
            {
                var info = await FromJson(meta);
                bucket.Import(info);
                return true;
            }
            return false;
        }

        public IEnumerable<IFileArg> TryUnpack(Bucket bucket)
        {
            foreach (var file in bucket.CheckProgress())
            {
                if (file is not Bucket.CompleteBucketFile cf)
                    continue;
                var text = cf.Text.ToString();
                if (string.IsNullOrWhiteSpace(text))
                    continue;
                cf.Text.Clear();
                yield return new MemFile(file.Meta, async (_, _) =>
                {
                    var decode = Encoder.Decode(text);
                    var decompress = await Compression.Decompress(decode);
                    return decompress;
                });
            }
        }

        public async IAsyncEnumerable<Stream> Pack(IFileArg[] input)
        {
            var allTmp = new StringBuilder();
            uint uncompressed = 0;
            uint compressed = 0;
            uint encoded = 0;
            var offsets = new uint[input.Length];
            var lengths = new uint[input.Length];
            var i = 0;
            foreach (var arg in input)
            {
                await using var raw = await arg.Read(MaxSize);
                uncompressed += (uint)arg.Size;
                await using var compress = await Compression.Compress(raw);
                compressed += (uint)compress.Length;
                var encode = Encoder.Encode(compress);
                encoded += (uint)encode.Length;
                offsets[i] = (uint)allTmp.Length;
                allTmp.Append(encode);
                lengths[i] = (uint)encode.Length;
                i++;
            }
            var pkgCount = (int)(allTmp.Length / (PkgSize * 1d)) + 1;
            var all = allTmp.ToString();

            var files = new PieceMeta[input.Length];
            for (var j = 0; j < input.Length; j++)
            {
                var item = input[j];
                files[j] = new PieceMeta
                {
                    Offset = (ushort)offsets[j],
                    Length = (ushort)lengths[j],
                    Meta = new FileMeta
                    {
                        Modified = item.LastModified,
                        Name = item.Name,
                        Size = item.Size,
                        Type = item.ContentType
                    }
                };
            }
            var content = new MetaContent
            {
                PieceLen = (ushort)PkgSize,
                Files = files,
                Stats = new PieceStats
                {
                    Raw = uncompressed,
                    Zip = compressed,
                    Code = encoded
                }
            };
            var meta = await ToJson(content);
            yield return await Barcoder.Write(meta.WriteAsStr());

            for (var j = 0; j < pkgCount; j++)
            {
                var part = all.Substr(j * PkgSize, PkgSize);
                var data = new DataPacket { Id = (ushort)j, Data = part };
                yield return await Barcoder.Write(data.WriteAsStr());
            }
        }

        private async Task<MetaPacket> ToJson(MetaContent content)
        {
            using var mem = new MemoryStream();
            await JsonSerializer.SerializeAsync(mem, content);
            mem.Position = 0;
            await using var compress = await Compression.Compress(mem);
            var encode = Encoder.Encode(compress);
            var meta = new MetaPacket { Id = 0, Data = encode };
            return meta;
        }

        private async Task<MetaContent> FromJson(MetaPacket packet)
        {
            await using var decode = Encoder.Decode(packet.Data);
            await using var decompress = await Compression.Decompress(decode);
            var content = await JsonSerializer.DeserializeAsync<MetaContent>(decompress);
            return content;
        }
    }
}