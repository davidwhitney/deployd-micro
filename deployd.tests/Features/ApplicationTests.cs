using System.IO.Abstractions;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.tests.Features
{
    [TestFixture]
    public class ApplicationTests
    {
        private Application _app;
        private Mock<IApplicationMap> _appMap;
        private Mock<IFileSystem> _fs;
        private Mock<ILog> _log;
        private Mock<IInstallationPadLock> _installationLock;
        private IInstanceConfiguration _instanceConfig;

        private const string FullPath = "c:\\fullPath";
        private const string ActiveDir = "c:\\active";
        private const string StagingDir = "c:\\staging";

        [SetUp]
        public void SetUp()
        {
            _appMap = new Mock<IApplicationMap>();
            _appMap.Setup(x => x.FullPath).Returns(FullPath);
            _appMap.Setup(x => x.Active).Returns(ActiveDir);
            _appMap.Setup(x => x.Staging).Returns(StagingDir);

            _fs = new Mock<IFileSystem>();
            _log = new Mock<ILog>();
            _installationLock = new Mock<IInstallationPadLock>();
            _instanceConfig = new InstanceConfiguration();

            _app = new Application(_appMap.Object, _fs.Object, _log.Object, _instanceConfig, _installationLock.Object);
        }

        [Test]
        public void ActivateStaging_MovedStagingToActiveDirectory()
        {
            _fs.Setup(x => x.Directory.Move(StagingDir, ActiveDir));

            _app.ActivateStaging();

            _fs.Verify(x=>x.Directory.Move(StagingDir, ActiveDir));
        }

        [Test]
        public void PruneBackups_LessThanTenBackups_NothingHappens()
        {
            var listOfDirectories = new[] { "App.1.0.0.0", "App.1.1.0.0" };
            _fs.Setup(x => x.Directory.GetDirectories(FullPath)).Returns(listOfDirectories);

            _app.PruneBackups();

            _fs.Verify(x => x.Directory.Delete(It.IsAny<string>(), true), Times.Never());
        }

        [Test]
        public void PruneBackups_MoreThanTenBackups_OldestItemRemoved()
        {
            var listOfDirectories = new[]
                {
                    "App.1.0.0.0", 
                    "App.1.1.0.0",
                    "App.1.2.0.0",
                    "App.1.3.0.0",
                    "App.1.4.0.0",
                    "App.1.5.0.0",
                    "App.1.6.0.0",
                    "App.1.7.0.0",
                    "App.1.8.0.0",
                    "App.1.9.0.0",
                    "App.1.10.0.0",
                };
            _fs.Setup(x => x.Directory.GetDirectories(FullPath)).Returns(listOfDirectories);

            _app.PruneBackups();

            _fs.Verify(x => x.Directory.Delete("App.1.0.0.0", true), Times.Once());
        }

        [Test]
        public void EnsureDataDirectoryExists_CreatesDirectoriesWhenTheyDontExist()
        {
            _fs.Setup(x => x.Directory.Exists(FullPath)).Returns(false);
            _fs.Setup(x => x.Directory.Exists(StagingDir)).Returns(false);
            _fs.Setup(x => x.Directory.CreateDirectory(FullPath));
            _fs.Setup(x => x.Directory.CreateDirectory(StagingDir));

            _app.EnsureDataDirectoriesExist();

            _fs.Verify(x => x.Directory.CreateDirectory(FullPath));
            _fs.Verify(x => x.Directory.CreateDirectory(StagingDir));
        }

        [Test]
        public void EnsureDataDirectoryExists_ChecksIfFullPathAndStagingDirExists()
        {
            _fs.Setup(x => x.Directory.Exists(FullPath)).Returns(true);
            _fs.Setup(x => x.Directory.Exists(StagingDir)).Returns(true);

            _app.EnsureDataDirectoriesExist();

            _fs.Verify(x => x.Directory.Exists(FullPath));
            _fs.Verify(x => x.Directory.Exists(StagingDir));
        }
    }
}
