using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class CompressionTaskSplitter : IBlockProcessingTaskSplitter
    {
        FileInfo InputFile { get; }
        public CompressionTaskSplitter(FileInfo inputFile)
        {
            InputFile = inputFile ?? throw new ArgumentNullException(nameof(inputFile));
        }

        public IList<IBlockProcessingTask> SplitToTasks()
        {
            return CreateCompressionTasks(InputFile).ToList();
        }

        static IEnumerable<IBlockProcessingTask> CreateCompressionTasks(FileInfo inputFile)
        {
            var blockSize = AppSettings.MaximumBlockSize;
            var blockIndex = 0;
            for (long offset = 0; offset < inputFile.Length; offset += blockSize)
            {
                var blockInfo = new BlockInfo
                {
                    BlockIndex = blockIndex++,
                    OriginalBlockOffset = offset,
                    OriginalBlockSize = Math.Min(inputFile.Length - offset, blockSize)
                };
                yield return new CompressionTask(() => inputFile.OpenRead(), blockInfo);
            }
        }
    }
}
