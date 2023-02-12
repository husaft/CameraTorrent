using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Components.Forms;

namespace CameraTorrent.Lib.Tests
{
    internal record FakeBrowserFile(
        string Name,
        DateTimeOffset LastModified,
        long Size,
        string ContentType,
        Func<long, Stream> Reader
    ) : IBrowserFile
    {
        public Stream OpenReadStream(long maxAllowedSize = 512000,
            CancellationToken token = new())
            => Reader(maxAllowedSize);
    }
}