using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public abstract class CompressionRunnerBase : IRunner
    {
        protected virtual IBlockProcessingTaskSplitter CreateTaskSplitter(Options options)
        {
            switch (options.Mode)
            {
                case Mode.Compress:
                    return new CompressionTaskSplitter(options.InputFile);
                case Mode.Decompress:
                    return new DecompressionTaskSplitter(options.InputFile);
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Mode));
            }
        }

        protected abstract IList<BlockInfo> RunTasks(IEnumerable<IBlockProcessingTask> tasks, Stream output);

        public void Run(Options options)
        {
            var taskSplitter = CreateTaskSplitter(options);
            var tasks = taskSplitter.SplitToTasks();
            using (var output = options.OutputFile.Open(FileMode.Create, FileAccess.Write))
            {
                using (var binaryWriter = new BinaryWriter(output))
                {
                    if (options.Mode == Mode.Compress)
                    {
                        binaryWriter.ReserveHeader(tasks.Count);

                        var blocks = RunTasks(tasks, output);

                        binaryWriter.WriteHeader(blocks);
                    }
                    else if (options.Mode == Mode.Decompress)
                    {
                        RunTasks(tasks, output);
                    }
                }
            }
        }
    }
}
