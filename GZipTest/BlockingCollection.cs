using System;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest
{
    /// <summary>
    /// Naive reimplementation of .Net 4.0 BlockingCollection
    /// </summary>
    /// <typeparam name="T">Collection element type</typeparam>
    public sealed class BlockingCollection<T>
    {
        readonly Queue<T> queue = new Queue<T>();
        volatile bool completed = false;

        public int BoundedCapacity { get; }

        public BlockingCollection(int boundedCapacity)
        {
            if (boundedCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(boundedCapacity));
            BoundedCapacity = boundedCapacity;
        }

        public void Add(T item)
        {
            EnsureNotCompleted();
            lock (queue)
            {
                while (queue.Count >= BoundedCapacity)
                {
                    Monitor.Wait(queue);
                }
                queue.Enqueue(item);
                Monitor.PulseAll(queue);
            }
        }

        public T Take()
        {
            lock (queue)
            {
                while (queue.Count == 0)
                {
                    EnsureNotCompleted();
                    Monitor.Wait(queue);
                }
                var result = queue.Dequeue();
                Monitor.PulseAll(queue);
                return result;
            }
        }

        void EnsureNotCompleted()
        {
            if (completed)
            {
                throw new InvalidOperationException();
            }
        }

        public void CompleteAdding()
        {
            lock (queue)
            {
                completed = true;
                Monitor.PulseAll(queue);
            }
        }

    }
}
