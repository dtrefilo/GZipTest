using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace GZipTest.Tests
{
    [TestClass]
    public class CompressionTaskSplitterTest : TestBase
    {
        static string testFile1;
        static string testFile2;
        static string testFile3;

        [ClassInitialize]
        public static void SuiteSetUp(TestContext testContext)
        {
            testFile1 = WriteStreamToTempFile("emergency-backup.log");
            testFile2 = WriteStreamToTempFile("empty.txt");
            testFile3 = WriteStreamToTempFile("binary.jar");
        }

        [ClassCleanup]
        public static void SuiteTearDown()
        {
            DeleteFileIfExists(testFile1);
            DeleteFileIfExists(testFile2);
            DeleteFileIfExists(testFile3);
        }

        [TestMethod]
        public void BadConstructorArguments()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new CompressionTaskSplitter(null))
                .ParamName.Should().Be("inputFile");
        }

        [TestMethod]
        public void TestSplitCompression1()
        {
            var splitter = new CompressionTaskSplitter(new FileInfo(testFile1));
            var tasks = splitter.SplitToTasks();
            tasks.Count.Should().Be(2);
            tasks[0].Should().BeOfType<CompressionTask>();
            tasks[0].BlockInfo.OriginalBlockSize.Should().Be(2097152);
            tasks[0].BlockInfo.OriginalBlockOffset.Should().Be(0);
            tasks[1].Should().BeOfType<CompressionTask>();
            tasks[1].BlockInfo.OriginalBlockSize.Should().Be(608610);
            tasks[1].BlockInfo.OriginalBlockOffset.Should().Be(2097152);
        }

        [TestMethod]
        public void TestSplitCompression2()
        {
            var splitter = new CompressionTaskSplitter(new FileInfo(testFile2));
            var tasks = splitter.SplitToTasks();
            tasks.Count.Should().Be(0);
        }

        [TestMethod]
        public void TestSplitCompression3()
        {
            var splitter = new CompressionTaskSplitter(new FileInfo(testFile3));
            var tasks = splitter.SplitToTasks();
            tasks.Count.Should().Be(1);
            tasks[0].Should().BeOfType<CompressionTask>();
            tasks[0].BlockInfo.OriginalBlockSize.Should().Be(308880);
            tasks[0].BlockInfo.OriginalBlockOffset.Should().Be(0);
        }
    }
}
