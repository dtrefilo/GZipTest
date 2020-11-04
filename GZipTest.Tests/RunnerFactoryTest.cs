using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GZipTest.Tests
{
    [TestClass]
    public class RunnerFactoryTest
    {
        [TestMethod]
        public void ShouldInstantiateSingleThreadedRunner()
        {
            new RunnerFactory(true).CreateRunner().Should().BeOfType<SingleThreadedCompressionRunner>();
        }

        [TestMethod]
        public void ShouldInstantiateMultiThreadedRunner()
        {
            new RunnerFactory(false).CreateRunner().Should().BeOfType<MultiThreadedCompressionRunner>();
        }
    }
}
