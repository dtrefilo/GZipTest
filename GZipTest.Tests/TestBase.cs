using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GZipTest.Tests
{
    public class TestBase
    {
        protected static Stream LoadTestStream(string streamName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("GZipTest.Tests.TestStreams." + streamName);
        }

        protected static string WriteStreamToTempFile(string streamName)
        {
            using (var stream = LoadTestStream(streamName))
            {
                var tempFileName = Path.GetTempFileName();
                using (var file = File.OpenWrite(tempFileName))
                {
                    stream.CopyTo(file);
                }
                return tempFileName;
            }
        }

        protected static string CompressToTempFile(string streamName)
        {
            var inputFile = WriteStreamToTempFile(streamName);
            var outputFile = Path.GetTempFileName();
            Options.TryParse(new[] { "compress", inputFile, outputFile }, Console.Out, out Options options);
            new SingleThreadedCompressionRunner().Run(options);
            DeleteFileIfExists(inputFile);
            return outputFile;
        }

        protected static void DeleteFileIfExists(string fileName)
        {
            if (fileName != null && File.Exists(fileName)) File.Delete(fileName);
        }

        public static IEnumerable<string> TestStreams => new[]
        {
            "emergency-backup.log",
            "empty.txt",
            "binary.jar"
        };
    }
}
