using System.IO;
using Microsoft.AspNetCore.Components.Forms;

namespace CameraTorrent.Lib.Tests
{
    internal static class TestTool
    {
        public static IBrowserFile Wrap(this FileInfo info)
        {
            var name = info.Name;
            var modified = info.LastWriteTime;
            var size = info.Length;
            var mime = $"binary/{info.Extension.TrimStart('.')}";
            Stream Read(long _) => File.OpenRead(info.FullName);
            return new FakeBrowserFile(name, modified, size, mime, Read);
        }
    }
}