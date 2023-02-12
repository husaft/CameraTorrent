using CameraTorrent.Lib.API;
using Microsoft.AspNetCore.Components.Forms;

namespace CameraTorrent.Util
{
    public static class WebTool
    {
        public static IFileArg Wrap(this IBrowserFile file)
        {
            return new WebFileArg(file);
        }
    }
}