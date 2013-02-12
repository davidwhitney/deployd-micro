using System.IO;
using System.IO.Abstractions;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;

namespace deployd.tests.Features.AppInstallation
{
    [TestFixture]
    public class InstallationPadLockTests
    {
        private Mock<IApplicationMap> _appMap;
        private Mock<IFileSystem> _fs;
        private InstallationPadLock _lock;
        private const string LockfileName = "c:\\LOCKFILE.txt";

        [SetUp]
        public void SetUp()
        {
            _appMap = new Mock<IApplicationMap>();
            _appMap.Setup(x => x.Lockfile).Returns(LockfileName);
            _fs = new Mock<IFileSystem>();
            _lock = new InstallationPadLock(_appMap.Object, _fs.Object);
        }

        [Test]
        public void LockAppInstallation_CreatesLockFileIfItDoesntExist()
        {
            _fs.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(false);

            _lock.LockAppInstallation();

            _fs.Verify(x=>x.File.WriteAllText(LockfileName, string.Empty));
        }

        [Test]
        public void LockAppInstallation_HoldsReadOnlyLockOnFile()
        {
            _fs.Setup(x => x.File.Exists(It.IsAny<string>())).Returns(true);

            _lock.LockAppInstallation();

            _fs.Verify(x => x.File.Open(LockfileName, FileMode.Open, FileAccess.Read, FileShare.None));
        }

        [Test]
        public void UnlockAppInstallation_ClosesAndDisposesOfLock()
        {
            var memStream = new MemoryStream();
            _appMap.Setup(x => x.Lock).Returns(memStream);
            _fs.Setup(x => x.File.Delete(LockfileName));

            _lock.UnlockAppInstallation();

            Assert.That(memStream.CanWrite, Is.False); // Is closed
            Assert.That(memStream.CanRead, Is.False);  // Is disposed
            _fs.VerifyAll();
        }
    }
}
