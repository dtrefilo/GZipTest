using System;

namespace GZipTest
{
    class Program
    {
        static void LogException(Exception ex)
        {
            Console.Error.WriteLine("Unexpected exception of type {0} occured", ex.GetType().FullName);
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);
            if (ex is AggregateException ae)
            {
                Console.Error.WriteLine("Inner:");
                foreach (var inner in ae.InnerExceptions) LogException(inner);
            }
        }

        static int Main(string[] args)
        {
            if (!Options.TryParse(args, Console.Out, out Options options)) return 1;
            try
            {
                new RunnerFactory(AppSettings.SingleThreaded).CreateRunner().Run(options);
            }
            catch (Exception ex)
            {
                LogException(ex);
                return 1;
            }
            return 0;
        }
    }
}
