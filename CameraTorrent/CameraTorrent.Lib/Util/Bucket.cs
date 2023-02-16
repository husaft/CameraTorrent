using System.Collections.Generic;
using System.Linq;
using CameraTorrent.Lib.API;
using CameraTorrent.Lib.Impl;
using CameraTorrent.Lib.Model;

namespace CameraTorrent.Lib.Util
{
    public sealed class Bucket
    {
        private List<(int id, int from, int end, int idx)> _toc;
        private IStringBuff[] _buff;
        private int[] _written;

        private void BuildToc(MetaContent info)
        {
            var toc = new List<(int id, int from, int end, int idx)>();
            var count = (int)(info.Stats.Code / (info.PieceLen * 1d)) + 1;
            for (var id = 0; id < count; id++)
            {
                var from = id * Info.PieceLen;
                var end = from + Info.PieceLen;

                for (var j = 0; j < info.Files.Length; j++)
                {
                    var piece = info.Files[j];
                    var pFrom = piece.Offset;
                    var pEnd = pFrom + piece.Length;

                    if (pEnd < from)
                        continue;
                    if (pFrom > end)
                        continue;

                    if (pFrom >= from && pEnd <= end)
                    {
                        toc.Add((id, pFrom, pEnd, j));
                        continue;
                    }

                    if (pFrom >= from && pEnd > end)
                    {
                        toc.Add((id, pFrom, end, j));
                        continue;
                    }

                    if (pFrom < from && pEnd <= end)
                    {
                        toc.Add((id, from, pEnd, j));
                        continue;
                    }

                    if (pFrom < from && pEnd > end)
                    {
                        toc.Add((id, from, end, j));
                    }
                }
            }
            _toc = toc;
        }

        public void Import(MetaContent info)
        {
            if (Info != null)
            {
                return;
            }
            BuildToc(Info = info);
            _buff = Info.Files
                .Select(p => (IStringBuff)new StringBuff(p.Length))
                .ToArray();
            _written = info.Files.Select(_ => 0).ToArray();
        }

        public void Import(DataPacket data)
        {
            var id = data.Id;
            var pOff = id * Info.PieceLen;
            var matches = _toc.Where(t => t.id == id).ToArray();
            foreach (var match in matches)
            {
                var fileId = match.idx;
                var piece = Info.Files[fileId];
                var offset = piece.Offset;
                var start = match.from - offset;
                var len = match.end - offset - start;
                var bld = _buff[fileId];
                var txt = data.Data.Substring(match.from - pOff, len);
                if (!bld.Replace(start, len, txt))
                    continue;
                _written[fileId] += txt.Length;
            }
        }

        public MetaContent Info { get; private set; }

        public IEnumerable<BucketFile> CheckProgress()
        {
            for (var i = 0; i < Info.Files.Length; i++)
            {
                var file = Info.Files[i];
                var len = file.Length;
                var writ = _written[i];
                if (len != writ)
                {
                    var progress = writ / (len * 1d) * 100d;
                    yield return new PartialBucketFile(progress, file.Meta);
                }
                else
                {
                    var bld = _buff[i];
                    yield return new CompleteBucketFile(bld, file.Meta);
                }
            }
        }

        public abstract record BucketFile(double Progress, FileMeta Meta);

        public record PartialBucketFile(double Progress, FileMeta Meta)
            : BucketFile(Progress, Meta);

        public record CompleteBucketFile(IStringBuff Text, FileMeta Meta)
            : BucketFile(100.0, Meta);
    }
}