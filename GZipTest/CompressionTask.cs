using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class CompressionTask : BlockProcessingTaskBase
    {
        public CompressionTask(Func<Stream> openInputStream, BlockInfo blockInfo)
            : base(openInputStream, blockInfo)
        {
        }

        protected override void ProcessStreams(Stream inputStream, MemoryStream tempStream)
        {
            inputStream.Seek(BlockInfo.OriginalBlockOffset, SeekOrigin.Begin);
            using (var gstream = new GZipStream(tempStream, CompressionMode.Compress, leaveOpen: true))
            {
                CopyStreams(inputStream, gstream, BlockInfo.OriginalBlockSize);
            }
        }

        protected override void WriteToOutput(MemoryStream tempStream, Stream outputStream)
        {
            BlockInfo.BlockOffset = outputStream.Position;
            BlockInfo.BlockSize = tempStream.Length;
            outputStream.Write(tempStream.GetBuffer(), 0, (int)tempStream.Length);
        }
    }
}
