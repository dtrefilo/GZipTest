using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class SingleThreadedCompressionRunner : CompressionRunnerBase
    {
        protected override IList<BlockInfo> RunTasks(IEnumerable<IBlockProcessingTask> tasks, Stream output)
        {
            var blocks = new List<BlockInfo>();
            foreach (var task in tasks)
            {
                task.Process();
                task.WriteResult(output);
                blocks.Add(task.BlockInfo);
                task.Dispose();
            }
            return blocks;
        }
    }
}
