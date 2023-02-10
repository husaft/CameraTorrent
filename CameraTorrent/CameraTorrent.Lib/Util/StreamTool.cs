using System;
using System.IO;

namespace CameraTorrent.Lib.Util
{
    public static class StreamTool
    {
        public static byte[] AsBytes(this Stream source)
        {
            return ((MemoryStream)source).ToArray();
        }

        public static string ToBase64Str(this Stream source, string mime = "image/png")
        {
            var buffer = ((MemoryStream)source).ToArray();
            var str = Convert.ToBase64String(buffer);
            return $"data:{mime};base64,{str}";
        }
    }
}