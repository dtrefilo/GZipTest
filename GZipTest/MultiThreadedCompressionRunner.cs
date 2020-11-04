using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace GZipTest
{
    public class MultiThreadedCompressionRunner : CompressionRunnerBase
    {
        IThrottling Throttling { get; }

        public MultiThreadedCompressionRunner(IThrottling throttling)
        {
            Throttling = throttling ?? throw new ArgumentNullException(nameof(throttling));
        }

        protected override IList<BlockInfo> RunTasks(IEnumerable<IBlockProcessingTask> tasks, Stream output)
        {
            // Arrange the stuff
            var errors = new List<Exception>();
            var blocks = new List<BlockInfo>();

            var unprocessedTasks = new Queue<IBlockProcessingTask>(tasks);
            var processedTasks = new BlockingCollection<IBlockProcessingTask>(Throttling.GetBoundedCapacity());

            void SafeExecute(Action action)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
            }

            var processorThreads = Enumerable.Range(0, Throttling.GetConcurrency()).Select(_ => new Thread(() =>
            {
                SafeExecute(() =>
                {
                    while (true)
                    {
                        IBlockProcessingTask task = null;
                        lock (unprocessedTasks)
                        {
                            if (unprocessedTasks.Count == 0) return; // all tasks processed
                            task = unprocessedTasks.Dequeue();
                        }
                        task.Process();
                        processedTasks.Add(task);
                        Console.WriteLine("[{0}] Block processed {1}", Thread.CurrentThread.ManagedThreadId, task.BlockInfo.BlockIndex);
                    }
                });
            })).ToList();
            var writerThread = new Thread(() =>
            {
                SafeExecute(() =>
                {
                    while (true)
                    {
                        IBlockProcessingTask task = null;
                        try
                        {
                            task = processedTasks.Take();
                        }
                        catch (InvalidOperationException)
                        {
                            return;
                        }
                        task.WriteResult(output);
                        blocks.Add(task.BlockInfo);
                        Console.WriteLine("[{0}] Block written {1}", Thread.CurrentThread.ManagedThreadId, task.BlockInfo.BlockIndex);
                        task.Dispose();
                    }
                });
            });

            // All systems go
            processorThreads.ForEach(t => t.Start());
            writerThread.Start();

            // Synchronize
            processorThreads.ForEach(t => t.Join());
            processedTasks.CompleteAdding();
            writerThread.Join();

            if (errors.Any()) throw new AggregateException("Some errors occured while processing blocks", errors);

            return blocks;
        }
    }
}
