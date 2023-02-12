namespace CameraTorrent.Lib.Model
{
    public static class Packets
    {
        public static (DataPacket, MetaPacket)? Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var data = new DataPacket();
            if (data.ReadStr(text))
                return (data, null);

            var meta = new MetaPacket();
            if (meta.ReadStr(text))
                return (null, meta);

            return null;
        }
    }
}