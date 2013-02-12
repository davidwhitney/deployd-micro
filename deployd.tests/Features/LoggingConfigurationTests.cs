using NUnit.Framework;
using deployd.Features;
using log4net;

namespace deployd.tests.Features
{
    [TestFixture]
    public class LoggingConfigurationTests
    {
        [Test]
        public void Test()
        {
            var log = LogManager.GetLogger("UnitTest");
            var configurer = new LoggingConfiguration(log);

            configurer.SetLogLevelToDebug();

            var lvl = ((log4net.Repository.Hierarchy.Logger) log.Logger).Level;
            Assert.That(lvl.Name, Is.EqualTo("DEBUG"));
        }
    }
}
