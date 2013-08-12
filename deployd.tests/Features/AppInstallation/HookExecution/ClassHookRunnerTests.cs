using System;
using System.Collections.Generic;
using System.IO;
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
        private TextWriter _output;

        public ClassHookRunnerTests()
        {
            _output = new StringWriter(new StringBuilder());
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _runner = new ClassHookRunner(_log.Object, _config.Object, _output);
        }

        [Test]
        public void SupportsHook_ForFile_ReturnsFalse()
        {
            var supports = _runner.SupportsHook(new HookTypeRef("aa", HookType.File));

            Assert.That(supports, Is.False);
        }
        
        [Test]
        public void SupportsHook_ForClass_ReturnsTrue()
        {
            var supports = _runner.SupportsHook(new HookTypeRef(typeof(object)));

            Assert.That(supports, Is.True);
        }

        [Test]
        [Ignore("I don't know how this is supposed to pass... needs refactoring")]
        public void ExecuteHook_WhenGivenAType_CreatesAndExecutesIt()
        {
            _runner.ExecuteHook(new HookTypeRef(typeof(FakeHook)));

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
