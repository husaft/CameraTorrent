using System;

namespace CameraTorrent.Lib.Model
{
    public sealed class FileMeta
    {
        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public long Size { get; set; }

        public string Type { get; set; }
    }
}