using System;
using Moq;
using NUnit.Framework;
using deployd.Features;
using deployd.Features.AppInstallation;
using deployd.Features.AppInstallation.HookExecution;

namespace deployd.tests.Features.AppInstallation
{
    [TestFixture]
    public class AppInstallationCommandTests
    {
        private Mock<IApplication> _app;
        private Mock<IInstallHookExecutor> _hookExecutor;
        private AppInstallationCommand _cmd;

        [SetUp]
        public void SetUp()
        {
            _app = new Mock<IApplication>();
            _app.Setup(x => x.IsStaged).Returns(true);
            _app.Setup(x => x.IsInstalled).Returns(true);
            _hookExecutor = new Mock<IInstallHookExecutor>();
            _cmd = new AppInstallationCommand(_app.Object, _hookExecutor.Object);
        }

        [Test]
        public void AppIsNotStaged_ThrowsInvalidOperationException()
        {
            _app.Setup(x => x.IsStaged).Returns(false);

            var ex = Assert.Throws<InvalidOperationException>(()=> _cmd.Execute());

            Assert.That(ex.Message, Is.EqualTo("Application isn't staged. Can't install."));
        }
        
        [Test]
        public void AppIsntCurrentlyInstalled_RunsFirstInstallHooks()
        {
            _app.Setup(x => x.IsInstalled).Returns(false);

            _hookExecutor.Setup(x => x.ExecuteFirstInstall());

            _cmd.Execute();

            _hookExecutor.VerifyAll();
        }
        
        [Test]
        public void AppIsInstalled_DoesntRunFirstInstallHooks()
        {
            _hookExecutor.Setup(x => x.ExecuteFirstInstall());

            _cmd.Execute();

            _hookExecutor.Verify(x=>x.ExecuteFirstInstall(), Times.Never());
        }

        [Test]
        public void AppIsInstalled_InstallRequested_RunsPreInstallHooks()
        {
            _hookExecutor.Setup(x => x.ExecutePreInstall());

            _cmd.Execute();

            _hookExecutor.VerifyAll();
        }

        [Test]
        public void AppIsInstalled_InstallRequested_RunsPostInstallHooks()
        {
            _hookExecutor.Setup(x => x.ExecutePostInstall());

            _cmd.Execute();

            _hookExecutor.VerifyAll();
        }

        [Test]
        public void AppIsInstalled_InstallRequested_RunsInstall()
        {
            _app.Setup(x => x.BackupAndInstall());

            _cmd.Execute();

            _app.VerifyAll();
        }

        [Test]
        public void AppIsNotInstalled_InstallRequested_RunsInstall()
        {
            _app.Setup(x => x.IsInstalled).Returns(false); 
            _app.Setup(x => x.BackupAndInstall());

            _cmd.Execute();

            _app.VerifyAll();
        }
    }
}
