using GZipTest.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public abstract class BlockProcessingTaskBase : IBlockProcessingTask
    {
        Func<Stream> OpenInputStream { get; }
        public BlockInfo BlockInfo { get; }

        MemoryStream tempStream;

        protected BlockProcessingTaskBase(Func<Stream> openInputStream, BlockInfo blockInfo)
        {
            OpenInputStream = openInputStream ?? throw new ArgumentNullException(nameof(openInputStream));
            BlockInfo = blockInfo ?? throw new ArgumentNullException(nameof(blockInfo));
        }

        protected abstract void ProcessStreams(Stream inputStream, MemoryStream tempStream);
        protected abstract void WriteToOutput(MemoryStream tempStream, Stream outputStream);

        protected static void CopyStreams(Stream sourceStream, Stream targetStream, long length)
        {
            var buffer = new byte[AppSettings.BufferSize];
            var leftRead = length;
            while (leftRead > 0)
            {
                var read = sourceStream.Read(buffer, 0, (int)Math.Min(buffer.Length, leftRead));
                leftRead -= read;
                targetStream.Write(buffer, 0, read);
            }
        }

        public void Process()
        {
            using (var inputStream = OpenInputStream.Invoke())
            {
                tempStream?.Dispose();
                tempStream = new MemoryStream();

                ProcessStreams(inputStream, tempStream);
            }
        }

        public void WriteResult(Stream outputStream)
        {
            if (tempStream == null) throw new InvalidOperationException("CompressionTask is in invalid state for this operation");
            WriteToOutput(tempStream, outputStream);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (tempStream != null)
            {
                tempStream.Dispose();
                tempStream = null;
            }
        }

        ~BlockProcessingTaskBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
