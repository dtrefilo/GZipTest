using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest.Interfaces
{
    public class BlockInfo
    {
        public int BlockIndex { get; set; }
        public long BlockOffset { get; set; }
        public long BlockSize { get; set; }
        public long OriginalBlockOffset { get; set; }
        public long OriginalBlockSize { get; set; }
    }

    public interface IBlockProcessingTask : IDisposable
    {
        BlockInfo BlockInfo { get; }
        void Process();
        void WriteResult(Stream outputStream);
    }
}
