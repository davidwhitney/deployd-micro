using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using deployd.Features.AppInstallation.HookExecution;
using deployd.Features.AppInstallation.Hooks;

namespace deployd.tests.Features.AppInstallation.HookExecution
{
    [TestFixture]
    public class InstallHookExecutorTests
    {
        private InstallHookExecutor _ihe;
        private Mock<IHookFinder> _finder;
        private List<IHookRunner> _hookRunners;
        private Mock<IHookRunner> _fakeHookRunner;

        [SetUp]
        public void SetUp()
        {
            _finder = new Mock<IHookFinder>();
            _fakeHookRunner = new Mock<IHookRunner>();
            _hookRunners = new List<IHookRunner> {_fakeHookRunner.Object};
            _fakeHookRunner.Setup(x => x.SupportsHook(It.IsAny<Hook>())).Returns(true);
            _ihe = new InstallHookExecutor(_finder.Object, _hookRunners);
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

        [Test]
        public void ExecuteFirstInstall_ExecutesDiscoveredHooks()
        {
            var fakeHook = new Hook("file.txt", HookType.File);
            _finder.Setup(x => x.DiscoverHooks()).Returns(new Hooks {FirstInstall = new List<Hook> {fakeHook}});

            _ihe.ExecuteFirstInstall();

            _fakeHookRunner.Verify(x => x.ExecuteHook(fakeHook), Times.Once());
        }

        [Test]
        public void ExecutePostInstall_ExecutesDiscoveredHooks()
        {
            var fakeHook = new Hook("file.txt", HookType.File);
            _finder.Setup(x => x.DiscoverHooks()).Returns(new Hooks {PostInstall = new List<Hook> {fakeHook}});

            _ihe.ExecutePostInstall();

            _fakeHookRunner.Verify(x => x.ExecuteHook(fakeHook), Times.Once());
        }

        [Test]
        public void ExecutePreInstall_ExecutesDiscoveredHooks()
        {
            var fakeHook = new Hook("file.txt", HookType.File);
            _finder.Setup(x => x.DiscoverHooks()).Returns(new Hooks {PreInstall = new List<Hook> {fakeHook}});

            _ihe.ExecutePreInstall();

            _fakeHookRunner.Verify(x => x.ExecuteHook(fakeHook), Times.Once());
        }
    }
}
