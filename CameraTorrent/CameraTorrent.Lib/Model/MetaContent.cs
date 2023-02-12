namespace CameraTorrent.Lib.Model
{
    public sealed class MetaContent
    {
        public ushort PieceLen { get; set; }

        public PieceMeta[] Files { get; set; }

        public PieceStats Stats { get; set; }
    }
}