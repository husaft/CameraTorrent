using System.IO;

namespace CameraTorrent.Lib.API
{
    public interface IEncoder
    {
        string Encode(Stream input);

        Stream Decode(string input);
    }
}