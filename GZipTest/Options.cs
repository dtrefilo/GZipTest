using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GZipTest
{

    public enum Mode
    {
        Compress,
        Decompress
    }

    public class Options
    {
        public Mode Mode { get; private set; }
        public FileInfo InputFile { get; private set; }
        public FileInfo OutputFile { get; private set; }

        static void PrintUsage(TextWriter output)
        {
            output.WriteLine("Usage: Compressor.exe <compress|decompress> <input-file> <output-file>");
        }

        static bool TryParseEnum<T>(string value, bool ignoreCase, out T enumValue)
            where T : struct
        {
            try
            {
                enumValue = (T)Enum.Parse(typeof(T), value, ignoreCase);
                return true;
            }
            catch (Exception)
            {
                enumValue = default(T);
                return false;
            }
        }

        public static bool TryParse(string[] args, TextWriter output, out Options options)
        {
            options = null;
            if (args.Length != 3)
            {
                output.WriteLine("Expected 3 argments, but received {0}", args.Length);
                PrintUsage(output);
                return false;
            }
            if (!TryParseEnum(args[0], true, out Mode mode))
            {
                output.WriteLine("Unrecognized mode parameter value: {0}", args[0]);
                PrintUsage(output);
                return false;
            }
            FileInfo inputFile, outputFile;
            try
            {
                inputFile = new FileInfo(args[1]);
                outputFile = new FileInfo(args[2]);
            }
            catch (Exception ex)
            {
                output.WriteLine("Unexpected exception of type {0} occured while processing arguments", ex.GetType().FullName);
                output.WriteLine(ex.StackTrace);
                return false;
            }
            if (!inputFile.Exists)
            {
                output.WriteLine("Input file cannot be found: {0}", args[1]);
                PrintUsage(output);
                return false;
            }
            options = new Options
            {
                Mode = mode,
                InputFile = inputFile,
                OutputFile = outputFile
            };
            return true;
        }
    }
}
