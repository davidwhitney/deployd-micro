using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.tests.Features.AppInstallation
{
    [TestFixture]
    public class InstallHookExecutorTests
    {
        private InstallHookExecutor _ihe;
        private Mock<IHookFinder> _finder;
        private Mock<ILog> _log;
        private Mock<IInstanceConfiguration> _config;

        [SetUp]
        public void SetUp()
        {
            _finder = new Mock<IHookFinder>();
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _ihe = new InstallHookExecutor(_finder.Object, _log.Object, _config.Object);
        }

        [Test]
        public void ExecuteFirstInstall_DiscoversHooks()
        {
            _finder.Setup(x => x.DiscoverHooks()).Returns(new Hooks());

            _ihe.ExecuteFirstInstall();

            _finder.VerifyAll();
        }

        [Test]
        public void ExecutePostInstall_DiscoversHooks()
        {
            _finder.Setup(x => x.DiscoverHooks()).Returns(new Hooks());

            _ihe.ExecutePostInstall();

            _finder.VerifyAll();
        }

        [Test]
        public void ExecutePreInstall_DiscoversHooks()
        {
            _finder.Setup(x => x.DiscoverHooks()).Returns(new Hooks());

            _ihe.ExecutePreInstall();

            _finder.VerifyAll();
        }
    }
}
