using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace GZipTest.Tests
{
    [TestClass]
    public class BlockingCollectionTest
    {
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void BadConstructorArguments(int boundedCapacity)
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new BlockingCollection<int>(boundedCapacity))
                .ParamName.Should().Be("boundedCapacity");
        }

        [TestMethod]
        public void ShouldAddAndTakeSingleItem()
        {
            var sut = new BlockingCollection<int>(1);
            sut.Add(12);
            sut.Take().Should().Be(12);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(10)]
        [DataRow(100)]
        public void ShouldStopAddingWhenFull(int boundedCapacity)
        {
            var sut = new BlockingCollection<int>(boundedCapacity);
            for (var i = 0; i < boundedCapacity; ++i) sut.Add(12); // Fill the collection
            var t = new Thread(() => sut.Add(13)); // Try to add one more item to the collection which is already full
            t.Start();

            t.Join(1000).Should().BeFalse("Should be blocked");

            sut.Take(); // Remove one item from the collection
            t.Join(1000).Should().BeTrue("Should be unblocked");
        }

        [TestMethod]
        public void ShouldThrowInvalidOperationExceptionWhenCompleted()
        {
            var sut = new BlockingCollection<int>(5);
            sut.CompleteAdding();
            Assert.ThrowsException<InvalidOperationException>(() => sut.Add(12));
            Assert.ThrowsException<InvalidOperationException>(() => sut.Take());
        }

        [TestMethod]
        public void ShouldAddAndTakeManyItemsMultithreaded()
        {
            var reference = Enumerable.Range(0, 10000).ToList();
            var output = new ConcurrentBag<int>();
            var input = new ConcurrentBag<int>(reference);
            var sut = new BlockingCollection<int>(5);

            var producerThreads = Enumerable.Range(0, 10).Select(_ => new Thread(() =>
            {
                while (input.TryTake(out int i))
                {
                    sut.Add(i);
                    Thread.Sleep(5);
                }
            })).ToList();

            var consumerThreads = Enumerable.Range(0, 10).Select(_ => new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        output.Add(sut.Take());
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                }
            })).ToList();

            producerThreads.ForEach(t => t.Start());
            consumerThreads.ForEach(t => t.Start());
            producerThreads.ForEach(t => t.Join());

            sut.CompleteAdding();
            consumerThreads.ForEach(t => t.Join());

            CollectionAssert.AreEquivalent(reference, output);
        }
    }
}
