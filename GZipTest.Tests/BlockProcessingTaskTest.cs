using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GZipTest.Tests
{
    [TestClass]
    public class BlockProcessingTaskTest : TestBase
    {
        [TestMethod]
        public void BadConstructorArguments()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CompressionTask(null, new Interfaces.BlockInfo()))
                .ParamName.Should().Be("openInputStream");
            Assert.ThrowsException<ArgumentNullException>(() => new CompressionTask(() => new MemoryStream(), null))
                .ParamName.Should().Be("blockInfo");

            Assert.ThrowsException<ArgumentNullException>(() => new DecompressionTask(null, new Interfaces.BlockInfo()))
                .ParamName.Should().Be("openInputStream");
            Assert.ThrowsException<ArgumentNullException>(() => new DecompressionTask(() => new MemoryStream(), null))
                .ParamName.Should().Be("blockInfo");
        }

        [DataTestMethod]
        [DataRow("emergency-backup.log")]
        [DataRow("empty.txt")]
        [DataRow("binary.jar")]
        public void ShouldCompressAndDecompressTestStreams(string streamName)
        {
            using (var original = new MemoryStream())
            {
                using (var testStream = LoadTestStream(streamName))
                {
                    testStream.CopyTo(original);
                }
                var blockInfo = new Interfaces.BlockInfo { OriginalBlockSize = original.Length };
                var compressionTask = new CompressionTask(() => LoadTestStream(streamName), blockInfo);
                compressionTask.Process();
                using (var compressed = new MemoryStream())
                {
                    compressionTask.WriteResult(compressed);

                    ReferenceEquals(compressionTask.BlockInfo, blockInfo).Should().BeTrue();
                    blockInfo.BlockSize.Should().Be(compressed.Length);

                    var decompressionTask = new DecompressionTask(() => new MemoryStream(compressed.GetBuffer(), 0, (int)compressed.Length, false), blockInfo);
                    decompressionTask.Process();
                    using (var decompressed = new MemoryStream())
                    {
                        decompressionTask.WriteResult(decompressed);

                        ReferenceEquals(decompressionTask.BlockInfo, blockInfo).Should().BeTrue();
                        blockInfo.OriginalBlockSize.Should().Be(decompressed.Length);

                        CollectionAssert.AreEqual(original.ToArray(), decompressed.ToArray());
                    }
                }
            }
        }

    }
}
