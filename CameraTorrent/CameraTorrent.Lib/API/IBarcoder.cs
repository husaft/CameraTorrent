using System.IO;
using System.Threading.Tasks;

namespace CameraTorrent.Lib.API
{
    public interface IBarcoder
    {
        Task<Stream> Write(string input);

        Task<string> Read(Stream input);
    }
}