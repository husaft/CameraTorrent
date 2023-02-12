using System;
using System.IO;
using System.Threading;

namespace CameraTorrent.Lib.API
{
    public interface IFileArg
    {
        string Name { get; }

        DateTimeOffset LastModified { get; }

        long Size { get; }

        string ContentType { get; }

        Stream OpenReadStream(long maxAllowedSize, CancellationToken token = default);
    }
}