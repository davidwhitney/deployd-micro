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
            _instanceConfig = new InstanceConfiguration {AppName = "MyApp"};
            _cmd = new AppLocatingCommand(_finders, _logger.Object, _instanceConfig);
        }

        [Test]
        public void InvokedWithConfig_LogsMessageIndicatingSearchTakingPlace()
        {
            _cmd.Execute();

            _logger.Verify(x => x.Info("Searching for package: " + _instanceConfig.AppName));
        }

        [Test]
        public void Invoked_ChecksAllApplicationLocatorsToSeeIfAnySupportConfiguredPathType()
        {
            var mockFinder = new Mock<IAppInstallationLocator>();
            mockFinder.Setup(x => x.SupportsPathType()).Returns(false);
            var mockFinder2 = new Mock<IAppInstallationLocator>();
            mockFinder2.Setup(x => x.SupportsPathType()).Returns(false);
            _finders.Add(mockFinder.Object);
            _finders.Add(mockFinder2.Object);

            _cmd.Execute();

            mockFinder.Verify(x=>x.SupportsPathType());
            mockFinder2.Verify(x=>x.SupportsPathType());
        }

        [Test]
        public void Invoked_WithAnActiveApplicationLocator_LocatorAskedToFindPackageByName()
        {
            var mockFinder = new Mock<IAppInstallationLocator>();
            _finders.Add(mockFinder.Object);
            mockFinder.Setup(x => x.SupportsPathType()).Returns(true);

            _cmd.Execute();

            mockFinder.Verify(x => x.CanFindPackageAsObject(_instanceConfig.AppName));
        }
        
        [Test]
        public void Invoked_LocatorFindsPackage_PackageLocationSetInConfigurationForUseBySubsequentCommands()
        {
            var package = new PackageLocation<object>();
            var mockFinder = new Mock<IAppInstallationLocator>();
            _finders.Add(mockFinder.Object);

            mockFinder.Setup(x => x.SupportsPathType()).Returns(true);
            mockFinder.Setup(x => x.CanFindPackageAsObject(_instanceConfig.AppName)).Returns(package);

            _cmd.Execute();

            Assert.That(_instanceConfig.PackageLocation, Is.EqualTo(package));
        }
        
        [Test]
        public void Invoked_LocatorFindsNothing_PackageLocationNotSet()
        {
            var package = new PackageLocation<object>();
            var mockFinder = new Mock<IAppInstallationLocator>();
            _finders.Add(mockFinder.Object);

            mockFinder.Setup(x => x.SupportsPathType()).Returns(true);
            mockFinder.Setup(x => x.CanFindPackageAsObject(_instanceConfig.AppName))
                      .Returns((PackageLocation<object>) null);

            _cmd.Execute();

            Assert.That(_instanceConfig.PackageLocation, Is.Null);
        }
    }
}
