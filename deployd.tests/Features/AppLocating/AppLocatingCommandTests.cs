using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.tests.Features.AppLocating
{
    [TestFixture]
    public class AppLocatingCommandTests
    {
        private List<IAppInstallationLocator> _finders;
        private AppLocatingCommand _cmd;
        private Mock<ILog> _logger;
        private IInstanceConfiguration _instanceConfig;

        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILog>();
            _finders = new List<IAppInstallationLocator>();
            _instanceConfig = new InstanceConfiguration();
            _cmd = new AppLocatingCommand(_finders, _logger.Object, _instanceConfig);
        }

        [Test]
        public void InvokedWithConfig_LogsMessageIndicatingSearchTakingPlace()
        {
            _instanceConfig.AppName = "MyApp";

            _cmd.Execute();

            _logger.Verify(x => x.Info("Searching for package: " + _instanceConfig.AppName));
        }
    }
}
