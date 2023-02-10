using System.IO;
using System.Threading.Tasks;

namespace CameraTorrent.Lib.API
{
    public interface ICompression
    {
        Task<Stream> Compress(Stream input);

        Task<Stream> Decompress(Stream input);
    }
}