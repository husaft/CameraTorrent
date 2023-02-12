namespace CameraTorrent.Lib.Model
{
    public abstract class Packet
    {
        public abstract string Header { get; }

        public abstract string WriteAsStr();

        public abstract bool ReadStr(string text);
    }
}