using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest.Interfaces
{
    public interface IThrottling
    {
        int GetBoundedCapacity();
        int GetConcurrency();
    }
}
