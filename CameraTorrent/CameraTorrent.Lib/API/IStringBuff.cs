namespace CameraTorrent.Lib.API
{
    public interface IStringBuff
    {
        bool Replace(int start, int length, string text);

        ushort Length { get; }

        void Clear();
    }
}