using System.IO;
using System.Threading.Tasks;
using CameraTorrent.Lib.API;
using CameraTorrent.Lib.Impl;
using Microsoft.AspNetCore.Components.Forms;

namespace CameraTorrent.Util
{
    public static class WebTool
    {
        public static IFileArg Wrap(this IBrowserFile file)
        {
            return new WebFileArg(file);
        }

        public static IBrowserFile Wrap(this byte[] data)
        {
            var mem = new MemFile(null, (_, _) =>
            {
                Stream stream = new MemoryStream(data);
                return Task.FromResult(stream);
            });
            return new WebFileArg(mem);
        }
    }
}