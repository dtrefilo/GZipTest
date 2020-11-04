using GZipTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GZipTest
{
    public class RunnerFactory : IRunnerFactory
    {
        readonly bool singleThreaded;

        public RunnerFactory(bool singleThreaded = false)
        {
            this.singleThreaded = singleThreaded;
        }

        public IRunner CreateRunner()
        {
            if (singleThreaded)
            {
                return new SingleThreadedCompressionRunner();
            }
            else
            {
                return new MultiThreadedCompressionRunner(new Throttling());
            }
        }
    }
}
