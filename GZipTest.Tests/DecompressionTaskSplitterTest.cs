using FluentAssertions;
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
    public class DecompressionTaskSplitterTest : TestBase
    {
        static string testFileNotCompressed1;
        static string testFileNotCompressed2;
        static string testFile1;
        static string testFile2;
        static string testFile3;

        [ClassInitialize]
        public static void SuiteSetUp(TestContext testContext)
        {
            testFileNotCompressed1 = WriteStreamToTempFile("binary.jar");
            testFileNotCompressed2 = WriteStreamToTempFile("empty.txt");
            testFile1 = CompressToTempFile("emergency-backup.log");
            testFile2 = CompressToTempFile("empty.txt");
            testFile3 = CompressToTempFile("binary.jar");
        }

        [ClassCleanup]
        public static void SuiteTearDown()
        {
            DeleteFileIfExists(testFileNotCompressed1);
            DeleteFileIfExists(testFileNotCompressed2);
            DeleteFileIfExists(testFile1);
            DeleteFileIfExists(testFile2);
            DeleteFileIfExists(testFile3);
        }

        [TestMethod]
        public void BadConstructorArguments()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DecompressionTaskSplitter(null))
                .ParamName.Should().Be("inputFile");
        }

        [TestMethod]
        public void TestSplitDecompressionNotCompressed1()
        {
            var splitter = new DecompressionTaskSplitter(new FileInfo(testFileNotCompressed1));
            Assert.ThrowsException<InvalidDataException>(() => splitter.SplitToTasks())
                .Message.Should().Be("Signature mismatch");
        }

        [TestMethod]
        public void TestSplitDecompressionNotCompressed2()
        {
            var splitter = new DecompressionTaskSplitter(new FileInfo(testFileNotCompressed2));
            Assert.ThrowsException<EndOfStreamException>(() => splitter.SplitToTasks());
        }

        [TestMethod]
        public void TestSplitDecompression1()
        {
            var splitter = new DecompressionTaskSplitter(new FileInfo(testFile1));
            var tasks = splitter.SplitToTasks();
            tasks.Count.Should().Be(2);
            tasks[0].Should().BeOfType<DecompressionTask>();
            tasks[0].BlockInfo.OriginalBlockSize.Should().Be(2097152);
            tasks[0].BlockInfo.OriginalBlockOffset.Should().Be(0);
            tasks[1].Should().BeOfType<DecompressionTask>();
            tasks[1].BlockInfo.OriginalBlockSize.Should().Be(608610);
            tasks[1].BlockInfo.OriginalBlockOffset.Should().Be(2097152);
        }

        [TestMethod]
        public void TestSplitCompression2()
        {
            var splitter = new DecompressionTaskSplitter(new FileInfo(testFile2));
            var tasks = splitter.SplitToTasks();
            tasks.Count.Should().Be(0);
        }

        [TestMethod]
        public void TestSplitCompression3()
        {
            var splitter = new DecompressionTaskSplitter(new FileInfo(testFile3));
            var tasks = splitter.SplitToTasks();
            tasks.Count.Should().Be(1);
            tasks[0].Should().BeOfType<DecompressionTask>();
            tasks[0].BlockInfo.OriginalBlockSize.Should().Be(308880);
            tasks[0].BlockInfo.OriginalBlockOffset.Should().Be(0);
        }

    }
}
