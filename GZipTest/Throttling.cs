using GZipTest.Interfaces;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class Throttling : IThrottling
    {
        public int GetBoundedCapacity()
        {
            var approximateBlockMemoryConsumption = (ulong)AppSettings.MaximumBlockSize * 10;
            var boundedCapacity = new ComputerInfo().AvailablePhysicalMemory / approximateBlockMemoryConsumption;
            return Math.Max(1, Math.Min((int)boundedCapacity, AppSettings.MaximumBoundedCapacity));
        }

        public int GetConcurrency()
        {
            var numAvailableCores = Environment.ProcessorCount * 2;
            return Math.Max(1, Math.Min(numAvailableCores, AppSettings.MaximumConcurrency));
        }

    }
}
