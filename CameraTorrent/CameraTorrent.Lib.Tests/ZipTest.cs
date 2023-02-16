using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using CameraTorrent.Lib.Util;
using CameraTorrent.Util;
using Xunit;

namespace CameraTorrent.Lib.Tests
{
    public class ZipTest
    {
        [Theory]
        [InlineData(new[] { "books.xml" }, new[] { 4548 })]
        [InlineData(new[] { "catalog.xml" }, new[] { 245 })]
        [InlineData(new[] { "icons1.png" }, new[] { 282 })]
        [InlineData(new[] { "icons2.png" }, new[] { 150 })]
        [InlineData(new[] { "icons3.png" }, new[] { 161 })]
        [InlineData(new[] { "sample.docx" }, new[] { 34375 })]
        [InlineData(new[] { "table.xls" }, new[] { 8704 })]
        [InlineData(new[] { "tondano.xml" }, new[] { 175592 })]
        [InlineData(new[] { "yukon.pdf" }, new[] { 20597 })]
        [InlineData(new[] { "icons3.png", "tondano.xml" }, new[] { 161, 175592 })]
        [InlineData(new[] { "catalog.xml", "books.xml" }, new[] { 245, 4548 })]
        [InlineData(new[] { "icons1.png", "icons3.png", "icons2.png" }, new[] { 282, 161, 150 })]
        [InlineData(new[] { "table.xls", "sample.docx" }, new[] { 8704, 34375 })]
        public async Task ShouldCompress(string[] fileNames, int[] sizes)
        {
            const string resDir = "Resources";
            var od = Directory.CreateDirectory($"_{resDir}").FullName;

            var raw = fileNames.Zip(sizes, (f, s) =>
                (fileName: Path.Combine(resDir, f), size: s)).ToArray();
            var prefix = string.Join(string.Empty, raw.Select(r =>
                Path.GetFileNameWithoutExtension(r.fileName)));

            Array.ForEach(raw, a =>
            {
                var info = new FileInfo(a.fileName);
                Assert.Equal(a.size, info.Length);
            });

            var inputs = raw.Select(r =>
                new FileInfo(r.fileName).Wrap().Wrap()).ToArray();

            var handle = new Torrent();
            var preDir = Path.Combine(od, prefix);
            var imageFiles = await WriteToImage(preDir, inputs, handle);
            var @new = await ReadFromImage(preDir, imageFiles, handle);

            foreach (var (first, second) in raw.Zip(@new))
            {
                var nInfo = new FileInfo(second);
                Assert.Equal(first.size, nInfo.Length);

                var alpha = await File.ReadAllBytesAsync(first.fileName);
                var beta = await File.ReadAllBytesAsync(second);
                Assert.Equal(alpha, beta);
            }
        }

        private static async Task<string[]> ReadFromImage(string prefix,
            string[] files, Torrent handle)
        {
            var bucket = new Bucket();
            foreach (var file in files)
            {
                await using var fileIn = File.OpenRead(file);
                var isGood = await handle.Unpack(fileIn, bucket);
                if (!isGood)
                    throw new InvalidOperationException(file);
            }

            var outs = new List<string>();
            foreach (var arg in handle.TryUnpack(bucket))
            {
                var fileOutName = $"{prefix}_{arg.Name}";
                await using var fileOut = File.Create(fileOutName);
                await using var data = await arg.Read(long.MaxValue);
                await data.CopyToAsync(fileOut);
                await fileOut.FlushAsync();
                outs.Add(fileOutName);
            }
            return outs.ToArray();
        }

        private static async Task<string[]> WriteToImage(string prefix,
            IFileArg[] inputs, Torrent handle)
        {
            var outs = new List<string>();
            var i = 0;
            await foreach (var code in handle.Pack(inputs))
            {
                var fileOutName = $"{prefix}_code_{i++:D3}.png";
                await using var image = code;

                await using var fileOut = File.Create(fileOutName);
                await image.CopyToAsync(fileOut);
                await fileOut.FlushAsync();
                outs.Add(fileOutName);
            }
            return outs.ToArray();
        }
    }
}