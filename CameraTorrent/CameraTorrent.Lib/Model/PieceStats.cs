namespace CameraTorrent.Lib.Model
{
    public sealed class PieceStats
    {
        public uint Raw { get; set; }

        public uint Zip { get; set; }

        public uint Code { get; set; }
    }
}