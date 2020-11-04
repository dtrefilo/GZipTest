using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class DecompressionTask : BlockProcessingTaskBase
    {
        public DecompressionTask(Func<Stream> openInputStream, BlockInfo blockInfo)
            : base(openInputStream, blockInfo)
        {
        }

        protected override void ProcessStreams(Stream inputStream, MemoryStream tempStream)
        {
            tempStream.Capacity = (int)BlockInfo.BlockSize;
            inputStream.Seek(BlockInfo.BlockOffset, SeekOrigin.Begin);
            using (var gstream = new GZipStream(inputStream, CompressionMode.Decompress, leaveOpen: true))
            {
                CopyStreams(gstream, tempStream, BlockInfo.OriginalBlockSize);
            }
        }

        protected override void WriteToOutput(MemoryStream tempStream, Stream outputStream)
        {
            if (BlockInfo.OriginalBlockSize != tempStream.Length) throw new InvalidDataException("Decompressed block size mismach");
            outputStream.Seek(BlockInfo.OriginalBlockOffset, SeekOrigin.Begin);
            outputStream.Write(tempStream.GetBuffer(), 0, (int)BlockInfo.OriginalBlockSize);
        }
    }
}
