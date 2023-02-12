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
        private readonly IBrowserFile _file;

        public WebFileArg(IBrowserFile file)
        {
            _file = file;
        }

        public string Name => _file.Name;
        public DateTimeOffset LastModified => _file.LastModified;
        public long Size => _file.Size;
        public string ContentType => _file.ContentType;

        public Stream OpenReadStream(long maxAllowedSize, CancellationToken token = default)
            => _file.OpenReadStream(maxAllowedSize, token);

        public Task<Stream> Read(long maxAllowedSize, CancellationToken token = default)
            => Task.FromResult(OpenReadStream(maxAllowedSize, token));
    }
}