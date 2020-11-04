using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IBlockProcessingTaskSplitter
    {
        IList<IBlockProcessingTask> SplitToTasks();
    }
}
