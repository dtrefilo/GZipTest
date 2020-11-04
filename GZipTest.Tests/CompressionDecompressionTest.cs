using FluentAssertions;
using GZipTest.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZipTest.Tests
{
    [TestClass]
    public class CompressionDecompressionTest : TestBase
    {
        void ShouldCompressAndDecompress(IRunner runner, string streamName)
        {
            var inputFileName = WriteStreamToTempFile(streamName);
            var compressedFile = Path.GetTempFileName();
            var decompressedFile = Path.GetTempFileName();

            try
            {
                Options.TryParse(new[] { "compress", inputFileName, compressedFile }, Console.Out, out Options options).Should().BeTrue();
                runner.Run(options);

                Options.TryParse(new[] { "decompress", compressedFile, decompressedFile }, Console.Out, out Options options2).Should().BeTrue();
                runner.Run(options2);

                var inputBytes = File.ReadAllBytes(inputFileName);
                var decompressedBytes = File.ReadAllBytes(decompressedFile);

                CollectionAssert.AreEqual(inputBytes, decompressedBytes);
            }
            finally
            {
                DeleteFileIfExists(inputFileName);
                DeleteFileIfExists(compressedFile);
                DeleteFileIfExists(decompressedFile);
            }
        }

        static IEnumerable<object[]> GetTestStreams() => TestStreams.Select(s => new object[] { s });

        [DataTestMethod]
        [DynamicData(nameof(GetTestStreams), DynamicDataSourceType.Method)]
        public void ShouldCompressAndDecompressSingleThreaded(string streamName)
        {
            ShouldCompressAndDecompress(new SingleThreadedCompressionRunner(), streamName);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetTestStreams), DynamicDataSourceType.Method)]
        public void ShouldCompressAndDecompressMultiThreaded(string streamName)
        {
            ShouldCompressAndDecompress(new MultiThreadedCompressionRunner(new Throttling()), streamName);
        }

        [TestMethod]
        public void BadConstructorArguments()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new MultiThreadedCompressionRunner(null))
                .ParamName.Should().Be("throttling");
        }
    }
}
