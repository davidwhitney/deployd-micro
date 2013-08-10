using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features;
using deployd.Features.AppInstallation;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.tests.Features
{
    [TestFixture]
    public class ApplicationTests
    {
        private Mock<IApplicationMap> _appMap;
        private Mock<ILog> _log;
        private Mock<IInstallationPadLock> _installationLock;
        private IInstanceConfiguration _instanceConfig;

        private const string InstallPath = "c:\\installPath";
        private const string StagingDir = "c:\\staging";
        private const string VersionFile = "c:\\installPath\\version";
        private const string CacheDir = "c:\\cache";

        [SetUp]
        public void SetUp()
        {
            _appMap = new Mock<IApplicationMap>();
            _appMap.Setup(x => x.InstallPath).Returns(InstallPath);
            _appMap.Setup(x => x.Staging).Returns(StagingDir);
            _appMap.Setup(x => x.VersionFile).Returns(VersionFile);
            _appMap.Setup(x => x.CachePath).Returns(CacheDir);

            _log = new Mock<ILog>();
            _installationLock = new Mock<IInstallationPadLock>();
            _instanceConfig = new InstanceConfiguration();
        }

        [Test]
        public void IsInstalled_IfVersionFileExists_ReturnsTrue()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {VersionFile, new MockFileData("1.0.0.0")},
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")}
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);
            Assert.That(app.IsInstalled, Is.True);
        }

        [Test]
        public void IsInstalled_IfVersionFileDoesntExist_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")}
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            Assert.That(app.IsInstalled, Is.False);
        }

        [Test]
        public void IsStaged_IfStagingDirectoryExists_ReturnsTrue()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")},
                    {StagingDir, new MockDirectoryData()},
                    {Path.Combine(StagingDir, "somefile.txt"), new MockFileData("sadfhsdf")},
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            Assert.That(app.IsStaged, Is.True);
        }

        [Test]
        public void IsStaged_IfStagingDirectoryDoesntExist_ReturnsFalse()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")},
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            Assert.That(app.IsStaged, Is.False);
        }

        [Test]
        public void ActivateStaging_MovedStagingToActiveDirectory()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")},
                    {Path.Combine(StagingDir, "SomeApplicationFile.dll"), new MockFileData(new byte[0])}
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.ActivateStaging();

            Assert.That(fileSystem.FileExists(Path.Combine(InstallPath, "SomeApplicationFile.dll")), Is.True);
        }

        [Test]
        public void BackupCurrentVersion_NotCurrentlyInstalled_DoesntMoveAnyDirectories()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {VersionFile, new MockFileData("1.0.0.0")},
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")}
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.BackupCurrentVersion();

            Assert.That(fileSystem.FileExists(Path.Combine(CacheDir, "1.0.0.0\\filetocopy.txt")), Is.True);
        }

        [Test]
        public void BackupCurrentVersion_AppIsInstalled_MovesCurrentAppToVersionedBackup()
        {

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {VersionFile, new MockFileData("1.0.0.0")},
                    {Path.Combine(InstallPath,"filetocopy.txt"), new MockFileData("file contents")}
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);
            app.BackupCurrentVersion();

            Assert.That(fileSystem.Directory.Exists(Path.Combine(CacheDir, "1.0.0.0")), Is.True);
        }

        [Test]
        public void BackupCurrentVersion_AlreadyABackupForThisVersion_MovesOldBackupFirst()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {VersionFile, new MockFileData("1.0.0.0")},
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")},
                    {Path.Combine(StagingDir, "SomeApplicationFile.dll"), new MockFileData(new byte[0])},
                    {Path.Combine(CacheDir, "1.0.0.0"), new MockDirectoryData()}
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.BackupCurrentVersion();

            Assert.That(fileSystem.DirectoryInfo.FromDirectoryName(CacheDir).GetDirectories().Length, Is.EqualTo(2));
        }

        [Test]
        public void PruneBackups_LessThanTenBackups_NothingHappens()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")},
                    {Path.Combine(StagingDir, "SomeApplicationFile.dll"), new MockFileData(new byte[0])},
                    {Path.Combine(CacheDir, "1.0.0.0\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.1\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.2\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.3\\somefile.txt"), new MockFileData("file contents")},
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.PruneBackups();

            Assert.That(fileSystem.DirectoryInfo.FromDirectoryName(CacheDir).GetDirectories().Length, Is.EqualTo(4));
        }

        [Test]
        public void PruneBackups_MoreThanTenBackups_OldestItemRemoved()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(InstallPath, "filetocopy.txt"), new MockFileData("file contents")},
                    {Path.Combine(StagingDir, "SomeApplicationFile.dll"), new MockFileData(new byte[0])},
                    {Path.Combine(CacheDir, "1.0.0.0\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.1\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.2\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.3\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.4\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.5\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.6\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.7\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.8\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.9\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.10\\somefile.txt"), new MockFileData("file contents")},
                    {Path.Combine(CacheDir, "1.0.0.11\\somefile.txt"), new MockFileData("file contents")},
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.PruneBackups();

            Assert.That(fileSystem.DirectoryInfo.FromDirectoryName(CacheDir).GetDirectories().Length, Is.EqualTo(10));
        }

        [Test]
        public void EnsureDataDirectoryExists_CreatesDirectoriesWhenTheyDontExist()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.EnsureDataDirectoriesExist();

            Assert.That(fileSystem.Directory.Exists(InstallPath));
            Assert.That(fileSystem.Directory.Exists(CacheDir));
            Assert.That(fileSystem.Directory.Exists(StagingDir));
            Assert.That((fileSystem.DirectoryInfo.FromDirectoryName(CacheDir).Attributes & FileAttributes.Hidden), Is.EqualTo(FileAttributes.Hidden));
        }

        [Test]
        public void EnsureDataDirectoryExists_ChecksIfInstallPathAndStagingDirExists()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {InstallPath, new MockDirectoryData()},
                {StagingDir, new MockDirectoryData()},
            });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            Assert.DoesNotThrow(app.EnsureDataDirectoriesExist);
        }

        [Test]
        public void LockForInstall_WhenCalled_InvokesVersionLock()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
            });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.LockForInstall();

            _installationLock.Verify(x=>x.LockAppInstallation());
        }

        [Test]
        public void WriteUpdatedManifest_WritesVersionFileWithPassedSemanticVersion()
        {
            const string version = "1.2.3.4";
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
            });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.WriteUpdatedManifest(version);

            Assert.That(fileSystem.FileExists(VersionFile), Is.True);
            Assert.That(fileSystem.File.ReadAllText(VersionFile), Is.StringMatching(version));
        }

        [Test]
        public void UpdateToLatestRevision_PerformsThreeStepInstall()
        {
            const string versionInstalled = "1.0.0.0";
            const string newVersion = "1.2.0.0";
            _instanceConfig.PackageLocation = new PackageLocation<object> {PackageVersion = newVersion};

            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {VersionFile, new MockFileData(versionInstalled)},
                {Path.Combine(InstallPath, "SomeApplicationFile.dll"), new MockFileData(new byte[0])}
            });

            var app = new Application(_appMap.Object, fileSystem, _log.Object, _instanceConfig, _installationLock.Object);

            app.EnsureDataDirectoriesExist();

            app.UpdateToLatestRevision();

            Assert.That(fileSystem.File.ReadAllText(VersionFile), Is.StringMatching(newVersion));
            Assert.That(fileSystem.FileExists(fileSystem.Path.Combine(CacheDir, versionInstalled + "\\SomeApplicationFile.dll")),Is.True);
        }
    }
}
