using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace CameraTorrent.Lib.Tests
{
    public class ZipTest
    {
        [Theory]
        [InlineData("catalog.xml", 245)]
        [InlineData("books.xml", 4548)]
        //[InlineData("logo.pdf", 437186)]
        //[InlineData("tondano.xml", 175592)]
        //[InlineData("yukon.pdf", 20597)]
        public async Task ShouldCompress(string fileName, int size)
        {
            const string resDir = "Resources";
            var rawFile = Path.Combine(resDir, fileName);
            Directory.CreateDirectory($"_{resDir}");

            var info = new FileInfo(rawFile);
            Assert.Equal(size, info.Length);

            var handle = new Torrent();
            var imageFile = await WriteToImage(rawFile, handle);
            var newFile = await ReadFromImage(imageFile, handle);

            var nInfo = new FileInfo(newFile);
            Assert.Equal(size, nInfo.Length);

            var alpha = await File.ReadAllBytesAsync(rawFile);
            var beta = await File.ReadAllBytesAsync(newFile);
            Assert.Equal(alpha, beta);
        }

        private static async Task<string> ReadFromImage(string file, Torrent handle)
        {
            await using var fileIn = File.OpenRead(file);
            await using var data = await handle.Unpack(fileIn);

            var fileOutName = $"_{file.Replace(".png", "")}";
            await using var fileOut = File.Create(fileOutName);
            await data.CopyToAsync(fileOut);
            await fileOut.FlushAsync();
            return fileOutName;
        }

        private static async Task<string> WriteToImage(string file, Torrent handle)
        {
            await using var fileIn = File.OpenRead(file);
            await using var image = await handle.Pack(fileIn);

            var fileOutName = $"{file}.png";
            await using var fileOut = File.Create(fileOutName);
            await image.CopyToAsync(fileOut);
            await fileOut.FlushAsync();
            return fileOutName;
        }
    }
}