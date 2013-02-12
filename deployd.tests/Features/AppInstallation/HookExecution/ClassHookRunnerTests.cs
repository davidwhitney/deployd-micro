using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation.HookExecution;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.tests.Features.AppInstallation.HookExecution
{
    [TestFixture]
    public class ClassHookRunnerTests
    {       
        private readonly Mock<ILog> _log;
        private readonly Mock<IInstanceConfiguration> _config;
        private readonly ClassHookRunner _runner;

        public ClassHookRunnerTests()
        {
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _runner = new ClassHookRunner(_log.Object, _config.Object);
        }

        [Test]
        public void SupportsHook_ForFile_ReturnsFalse()
        {
            var supports = _runner.SupportsHook(new Hook("aa", HookType.File));

            Assert.That(supports, Is.False);
        }
        
        [Test]
        public void SupportsHook_ForClass_ReturnsTrue()
        {
            var supports = _runner.SupportsHook(new Hook(typeof(object)));

            Assert.That(supports, Is.True);
        }

        [Test]
        public void ExecuteHook_WhenGivenAType_CreatesAndExecutesIt()
        {
            _runner.ExecuteHook(new Hook(typeof(FakeHook)));

            Assert.That(FakeHook.Executed, Is.True);
            Assert.That(FakeHook.Config, Is.EqualTo(_config.Object));
        }

        public class FakeHook : IHook
        {
            public static bool Executed { get; set; }
            public static IInstanceConfiguration Config { get; set; }

            public void Execute(IInstanceConfiguration config)
            {
                Executed = true;
                Config = config;
            }
        }
    }
}
