using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using deployd.Features.FeatureSelection;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;
using log4net;

namespace deployd.tests.Features.AppLocating
{
    [TestFixture]
    public class FileSystemAppInstallationLocatorTests
    {
        private DeploydConfiguration _config;
        private readonly Mock<IPackageSourceConfiguration> _packageSourceConfiguration = new Mock<IPackageSourceConfiguration>();
        private Mock<IFileSystem> _fs;
        private Mock<ILog> _log;
        private FileSystemAppInstallationLocator _locator;
        private List<string> _packageList;

        [SetUp]
        public void SetUp()
        {
            _log = new Mock<ILog>();
            _fs = new Mock<IFileSystem>();
            _packageList = new List<string>();
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>(), SearchOption.AllDirectories))
               .Returns(_packageList.ToArray);
            _locator = new FileSystemAppInstallationLocator(_packageSourceConfiguration.Object, _fs.Object, _log.Object);
        }

        [Test]
        public void SupportsPathType_WhenPackageSourceIsDirectoryAndDirectoryExists_ReturnsTrue()
        {
            _packageSourceConfiguration.Setup(x => x.PackageSource).Returns("c:\\myrepo");
            _fs.Setup(x => x.Directory.Exists("c:\\myrepo")).Returns(true);

            Assert.That(_locator.SupportsPathType(), Is.True);
        }

        [Test]
        public void SupportsPathType_WhenPackageSourceIsDirectoryAndDirectoryDoesntExists_ReturnsFalse()
        {
            _packageSourceConfiguration.Setup(x => x.PackageSource).Returns("c:\\myrepo");
            _fs.Setup(x => x.Directory.Exists("c:\\myrepo")).Returns(false);
            
            Assert.That(_locator.SupportsPathType(), Is.False);
        }

        [Test]
        public void CanFindPackage_WhenPackageIsFound_PackageIsReturnedInWrapper()
        {
            _packageList.Add("c:\\appName.1.0.0.0.zip");

            var returnedPackage = _locator.CanFindPackage("appName");

            Assert.That(returnedPackage.PackageDetails.PathAndFileName, Is.EqualTo("c:\\appName.1.0.0.0.zip"));
        }

        [Test]
        public void CanFindPackage_WhenMultiplePackagesFound_NewestPackageIsReturnedInWrapper()
        {
            _packageList.Add("c:\\appName.1.2.0.0.zip");
            _packageList.Add("c:\\appName.1.5.0.0.zip");
            _packageList.Add("c:\\appName.1.0.0.0.zip");

            var returnedPackage = _locator.CanFindPackage("appName");

            Assert.That(returnedPackage.PackageDetails.PathAndFileName, Is.EqualTo("c:\\appName.1.5.0.0.zip"));
        }

        [Test]
        public void CanFindPackageAsObject_WhenPackageIsFound_PackageIsReturnedInWrapper()
        {
            _packageList.Add("c:\\appName.1.2.0.0.zip");
            _packageList.Add("c:\\appName.1.5.0.0.zip");
            _packageList.Add("c:\\appName.1.0.0.0.zip");

            var returnedPackage = _locator.CanFindPackageAsObject("appName");

            var details = (PackagePointer)returnedPackage.PackageDetails;
            Assert.That(details.PathAndFileName, Is.EqualTo("c:\\appName.1.5.0.0.zip"));
        }

        [Test]
        public void CanFindPackage_FileSystemThrowsException_LogsErrorAndReturnsNull()
        {
            var ex = new Exception();
            _packageSourceConfiguration.Setup(x => x.PackageSource).Returns("c:\\myrepo");
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>(), SearchOption.AllDirectories))
               .Throws(ex);

            var returnedPackage = _locator.CanFindPackage("appName");

            _log.Verify(x=>x.Error("Could not get package from file system", ex));
            Assert.That(returnedPackage, Is.Null);
        }

        [Test]
        public void CanFindPackage_DirectorySearchReturnsNull_ReturnsNull()
        {
            _packageSourceConfiguration.Setup(x => x.PackageSource).Returns("c:\\myrepo");
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>(), SearchOption.AllDirectories))
               .Returns((string[])null);

            var returnedPackage = _locator.CanFindPackage("appName");

            Assert.That(returnedPackage, Is.Null);
        }

        [Test]
        public void CanFindPackage_DirectorySearchReturnsEmptyCollection_ReturnsNull()
        {
            _packageSourceConfiguration.Setup(x => x.PackageSource).Returns("c:\\myrepo");
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), It.IsAny<string>(), SearchOption.AllDirectories))
               .Returns(new string[0]);

            var returnedPackage = _locator.CanFindPackage("appName");

            Assert.That(returnedPackage, Is.Null);
        }
    }
}