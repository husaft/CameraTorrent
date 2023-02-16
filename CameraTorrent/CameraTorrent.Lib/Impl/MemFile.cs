using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using CameraTorrent.Lib.Model;

namespace CameraTorrent.Lib.Impl
{
    public class MemFile : IFileArg
    {
        private readonly FileMeta _meta;
        private readonly Func<long, CancellationToken, Task<Stream>> _get;

        public MemFile(FileMeta meta, Func<long, CancellationToken, Task<Stream>> get)
        {
            _meta = meta;
            _get = get;
        }

        public string Name => _meta.Name;
        public DateTimeOffset LastModified => _meta.Modified;
        public long Size => _meta.Size;
        public string ContentType => _meta.Type;

        public Task<Stream> Read(long maxAllowedSize, CancellationToken token = default)
            => _get(maxAllowedSize, token);
    }
}