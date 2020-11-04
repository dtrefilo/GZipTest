using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public static class AppSettings
    {
        public static long MaximumBlockSize { get; private set; } = 2 * 1024 * 1024;
        public static int MaximumBoundedCapacity { get; private set; } = 128;
        public static int MaximumConcurrency { get; private set; } = 256;
        public static int BufferSize { get; private set; } = 4 * 1024;
        public static bool SingleThreaded { get; private set; } = false;

        static AppSettings()
        {
            if (ulong.TryParse(ConfigurationManager.AppSettings[nameof(MaximumBlockSize)], out ulong maxBlockSize)) { MaximumBlockSize = (long)maxBlockSize; }
            if (uint.TryParse(ConfigurationManager.AppSettings[nameof(MaximumBoundedCapacity)], out uint maximumBoundedCapacity)) { MaximumBoundedCapacity = (int)maximumBoundedCapacity; }
            if (uint.TryParse(ConfigurationManager.AppSettings[nameof(MaximumConcurrency)], out uint maximumConcurrency)) { MaximumConcurrency = (int)maximumConcurrency; }
            if (uint.TryParse(ConfigurationManager.AppSettings[nameof(BufferSize)], out uint bufferSize)) { BufferSize = (int)bufferSize; }
            if (bool.TryParse(ConfigurationManager.AppSettings[nameof(SingleThreaded)], out bool singleThreaded)) { SingleThreaded = singleThreaded; }
        }
    }
}
