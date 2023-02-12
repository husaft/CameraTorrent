using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CameraTorrent.Lib.API
{
    public interface IFileArg
    {
        string Name { get; }

        DateTimeOffset LastModified { get; }

        long Size { get; }

        string ContentType { get; }

        Task<Stream> Read(long maxAllowedSize, CancellationToken token = default);
    }
}