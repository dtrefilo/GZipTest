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
    public class OptionsTest : TestBase
    {
        [DataTestMethod]
        [DataRow("compress", Mode.Compress)]
        [DataRow("decompress", Mode.Decompress)]
        [DataRow("cOmPrEsS", Mode.Compress)]
        [DataRow("dEcOmPrEsS", Mode.Decompress)]
        public void ShouldCreateValidOptions(string modeString, Mode expectedMode)
        {
            var inputFile = WriteStreamToTempFile("empty.txt");
            var outputFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Options.TryParse(new[] { modeString, inputFile, outputFile }, Console.Out, out Options options).Should().BeTrue();
            options.Should().NotBeNull();
            options.Mode.Should().Be(expectedMode);
            options.InputFile.Should().NotBeNull();
            options.InputFile.FullName.Should().Be(inputFile);
            options.OutputFile.FullName.Should().Be(outputFile);
        }

        public static IEnumerable<object[]> GetBadArguments()
        {
            yield return new object[] { new string[] { }, "Expected 3 argments, but received 0" };
            yield return new object[] { new string[] { "1" }, "Expected 3 argments, but received 1" };
            yield return new object[] { new string[] { "1", "2" }, "Expected 3 argments, but received 2" };
            yield return new object[] { new string[] { "1", "2", "3", "4" }, "Expected 3 argments, but received 4" };
            yield return new object[] { new string[] { "uncompress", "C:\\in.gz", "C:\\out.bin" }, "Unrecognized mode parameter value: uncompress" };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetBadArguments), DynamicDataSourceType.Method)]
        public void ShouldNotCreateOptionsFromBadArguments(string[] args, string expectedMessage)
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    Options.TryParse(args, writer, out Options options).Should().BeFalse();
                    options.Should().BeNull();
                }
                Encoding.UTF8.GetString(stream.ToArray()).Should().Contain(expectedMessage);
            }
        }

        [TestMethod]
        public void ShouldNotCreateOptionsWithBadFileName()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    Options.TryParse(new[] { "compress", "", "" }, writer, out Options options).Should().BeFalse();
                    options.Should().BeNull();
                }
                Encoding.UTF8.GetString(stream.ToArray()).Should().Contain("Unexpected exception of type System.ArgumentException occured while processing arguments");
            }
        }

        [TestMethod]
        public void ShouldNotCreateOptionsIfInputFileDoesNotExist()
        {
            var inputFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var outputFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    Options.TryParse(new[] { "compress", inputFile, outputFile }, writer, out Options options).Should().BeFalse();
                    options.Should().BeNull();
                }
                Encoding.UTF8.GetString(stream.ToArray()).Should().Contain($"Input file cannot be found: {inputFile}");
            }
        }
    }
}
