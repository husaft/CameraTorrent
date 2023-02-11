﻿// ReSharper disable InconsistentNaming

using System.IO;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace CameraTorrent.Lib.Impl
{
    public sealed class QRBarcoder : IBarcoder
    {
        public int Size { get; set; } = 600;

        public async Task<Stream> Write(string input)
        {
            var mem = new MemoryStream();
            var qr = new ZXing.ImageSharp.BarcodeWriter<Rgb24>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    ErrorCorrection = ErrorCorrectionLevel.L,
                    PureBarcode = true,
                    Height = Size,
                    Width = Size,
                    Margin = 1
                }
            };
            using var img = qr.Write(input);
            await img.SaveAsPngAsync(mem);
            await mem.FlushAsync();
            mem.Position = 0L;
            return mem;
        }

        public async Task<string> Read(Stream input)
        {
            using var img = await Image.LoadAsync<Rgb24>(input);
            var qr = new ZXing.ImageSharp.BarcodeReader<Rgb24>();
            var result = qr.Decode(img);
            var text = result.Text;
            return text;
        }
    }
}