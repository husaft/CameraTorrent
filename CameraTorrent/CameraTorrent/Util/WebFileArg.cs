using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using Microsoft.AspNetCore.Components.Forms;

namespace CameraTorrent.Util
{
    internal sealed class WebFileArg : IFileArg, IBrowserFile
    {
        private readonly IBrowserFile? _bFile;
        private readonly IFileArg? _aFile;

        public WebFileArg(IBrowserFile file)
        {
            _bFile = file;
        }

        public WebFileArg(IFileArg file)
        {
            _aFile = file;
        }

        public string Name => _bFile?.Name ?? _aFile!.Name;
        public DateTimeOffset LastModified => _bFile?.LastModified ?? _aFile!.LastModified;
        public long Size => _bFile?.Size ?? _aFile!.Size;
        public string ContentType => _bFile?.ContentType ?? _aFile!.ContentType;

        public Stream OpenReadStream(long maxAllowedSize, CancellationToken token = default)
            => _aFile?.Read(maxAllowedSize, token).GetAwaiter().GetResult() ??
               _bFile!.OpenReadStream(maxAllowedSize, token);

        public Task<Stream> Read(long maxAllowedSize, CancellationToken token = default)
            => _aFile?.Read(maxAllowedSize, token) ??
               Task.FromResult(_bFile!.OpenReadStream(maxAllowedSize, token));
    }
}