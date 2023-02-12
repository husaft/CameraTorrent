namespace CameraTorrent.Lib.Model
{
    public sealed class PieceMeta
    {
        public FileMeta Meta { get; set; }

        public ushort Offset { get; set; }

        public string[] Pieces { get; set; }
    }
}