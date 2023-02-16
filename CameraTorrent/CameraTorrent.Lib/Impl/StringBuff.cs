using System.Linq;
using CameraTorrent.Lib.API;

namespace CameraTorrent.Lib.Impl
{
    public sealed class StringBuff : IStringBuff
    {
        public StringBuff(ushort length)
        {
            Array = new char?[Length = length];
        }

        public ushort Length { get; }
        private char?[] Array { get; }

        public bool Replace(int start, int length, string text)
        {
            var isNew = true;
            for (var i = 0; i < length; i++)
            {
                var idx = start + i;
                if (isNew && Array[idx] != null)
                    isNew = false;
                Array[idx] = text[i];
            }
            return isNew;
        }

        public void Clear()
        {
            for (var i = 0; i < Length; i++)
                Array[i] = null;
        }

        public override string ToString()
        {
            var l = Array.Select(a => a.GetValueOrDefault()).ToArray();
            var txt = new string(l);
            return txt;
        }
    }
}