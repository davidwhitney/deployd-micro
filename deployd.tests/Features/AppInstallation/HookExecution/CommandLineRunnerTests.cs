using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using deployd.Features.AppInstallation.HookExecution;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.tests.Features.AppInstallation.HookExecution
{
    [TestFixture]
    public class CommandLineRunnerTests
    {
        private readonly Mock<ILog> _log;
        private readonly Mock<IInstanceConfiguration> _config;
        private readonly Mock<IFileSystem> _fs;
        private readonly CommandLineRunner _runner;
        const string HookFileName = "internal-use-only.ps1";

        public CommandLineRunnerTests()
        {
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _fs = new Mock<IFileSystem>();
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object);
        }
        
        [Test]
        public void CopyVariablesToEnvironment_MapsApplicationMapToEnvironment()
        {
            var map = new ApplicationMap("testApp", "c:\\full\\path");
            _config.Setup(x => x.ApplicationMap).Returns(map);
            var startInfo = new ProcessStartInfo { FileName = HookFileName };

            _runner.CopyVariablesToEnvironment(startInfo);
            
            const string prefix = "Deployd.";
            var countOfDeploydKeys = startInfo.EnvironmentVariables.Keys.Cast<string>().Count(var => var.StartsWith(prefix));

            Assert.That(startInfo.EnvironmentVariables[prefix + "Active"], Is.EqualTo(map.Active));
            Assert.That(startInfo.EnvironmentVariables[prefix + "AppName"], Is.EqualTo(map.AppName));
            Assert.That(startInfo.EnvironmentVariables[prefix + "FullPath"], Is.EqualTo(map.FullPath));
            Assert.That(startInfo.EnvironmentVariables[prefix + "Lock"], Is.EqualTo(map.Lock));
            Assert.That(startInfo.EnvironmentVariables[prefix + "Lockfile"], Is.EqualTo(map.Lockfile));
            Assert.That(startInfo.EnvironmentVariables[prefix + "Staging"], Is.EqualTo(map.Staging));
            Assert.That(startInfo.EnvironmentVariables[prefix + "VersionFile"], Is.EqualTo(map.VersionFile));
            Assert.That(countOfDeploydKeys, Is.GreaterThan(0));
        }

        [TestCase("rb", "ruby")]
        [TestCase("py", "python")]
        [TestCase("cgi", "perl")]
        [TestCase("php", "php")]
        [TestCase("js", "node")]
        public void PrefixCommonScriptRuntimes_WhenGivenKnownScript_PrefixesKnownInterpreter(string extension, string interpreterExpected)
        {
            var script = "script." + extension;
            var startInfo = new ProcessStartInfo { FileName = script };

            _runner.PrefixCommonScriptRuntimes(script, startInfo);

            Assert.That(startInfo.FileName, Is.EqualTo(interpreterExpected + " " + script));
        }

        [Test]
        public void SupportsHook_ForFile_ReturnsTrue()
        {
            var path = new Mock<PathWrapper>();
            path.Setup(p => p.GetExtension(It.IsAny<string>())).Returns(".rb");
            _fs.Setup(x => x.Path).Returns(path.Object);
            var supports = _runner.SupportsHook(new HookTypeRef("aa", HookType.File));

            Assert.That(supports, Is.True);
        }

        [Test]
        public void SupportsHook_ForClass_ReturnsFalse()
        {
            var supports = _runner.SupportsHook(new HookTypeRef(typeof(object)));

            Assert.That(supports, Is.False);
        }
        
        [Test]
        public void ExecuteHook_WhenGivenAProcessThatExitsCleanly_DoesntThrow()
        {
            _runner.ExecuteHook(new HookTypeRef("cmd", HookType.File), "/c exit 0");
        }

        [Test]
        public void ExecuteHook_WhenGivenAProcessThatExitsWithANoneZeroCode_Throws()
        {
            var ex = Assert.Throws<HookFailureException>(()=> _runner.ExecuteHook(new HookTypeRef("cmd", HookType.File), "/c exit -300"));

            Assert.That(ex.HookFile, Is.EqualTo("cmd"));
            Assert.That(ex.ExitCode, Is.EqualTo(-300));
        }
    }
}
