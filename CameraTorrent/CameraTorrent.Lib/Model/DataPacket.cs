using System;

namespace CameraTorrent.Lib.Model
{
    public class DataPacket : Packet
    {
        public override string Header => "C1D";

        public ushort Id { get; set; }
        public string Data { get; set; }

        public override string WriteAsStr()
        {
            return $"{Header}{Id:x4}{Data}";
        }

        public override bool ReadStr(string text)
        {
            if (!text.StartsWith(Header))
                return false;

            var tmp = text[Header.Length..];
            var part = tmp[..4];
            Id = Convert.ToUInt16(part, 16);
            Data = tmp[part.Length..];

            return true;
        }
    }
}