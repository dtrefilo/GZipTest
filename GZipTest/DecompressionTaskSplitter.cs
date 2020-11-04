using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class DecompressionTaskSplitter : IBlockProcessingTaskSplitter
    {
        FileInfo InputFile { get; }
        public DecompressionTaskSplitter(FileInfo inputFile)
        {
            InputFile = inputFile ?? throw new ArgumentNullException(nameof(inputFile));
        }

        public IList<IBlockProcessingTask> SplitToTasks()
        {
            return CreateDecompressionTasks(InputFile).ToList();
        }

        static IEnumerable<IBlockProcessingTask> CreateDecompressionTasks(FileInfo inputFile)
        {
            using (var stream = inputFile.OpenRead())
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    var blocks = binaryReader.ReadHeader();
                    foreach (var block in blocks)
                    {
                        yield return new DecompressionTask(() => inputFile.OpenRead(), block);
                    }
                }
            }
        }
    }
}
