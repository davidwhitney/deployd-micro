using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
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
        private CommandLineRunner _runner;
        const string HookFileName = "internal-use-only.ps1";
        private readonly TextWriter _output = new StringWriter(new StringBuilder());

        public CommandLineRunnerTests()
        {
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _fs = new Mock<IFileSystem>();
            
        }
        
        [Test]
        public void CopyVariablesToEnvironment_MapsApplicationMapToEnvironment()
        {
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object, _output);
            var map = new ApplicationMap("testApp", "c:\\install\\path");
            _config.Setup(x => x.ApplicationMap).Returns(map);
            var startInfo = new ProcessStartInfo { FileName = HookFileName };

            _runner.CopyVariablesToEnvironment(startInfo);
            
            const string prefix = "Deployd.";
            var countOfDeploydKeys = startInfo.EnvironmentVariables.Keys.Cast<string>().Count(var => var.StartsWith(prefix));

            Assert.That(startInfo.EnvironmentVariables[prefix + "AppName"], Is.EqualTo(map.AppName));
            Assert.That(startInfo.EnvironmentVariables[prefix + "InstallPath"], Is.EqualTo(map.InstallPath));
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
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object, _output);
            var script = "script." + extension;
            var startInfo = new ProcessStartInfo { FileName = script };

            _runner.PrefixCommonScriptRuntimes(script, startInfo);

            Assert.That(startInfo.FileName, Is.EqualTo(interpreterExpected + " " + script));
        }

        [Test]
        public void SupportsHook_ForFile_ReturnsTrue()
        {
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object, _output);
            var path = new Mock<PathWrapper>();
            path.Setup(p => p.GetExtension(It.IsAny<string>())).Returns(".rb");
            _fs.Setup(x => x.Path).Returns(path.Object);
            var supports = _runner.SupportsHook(new HookTypeRef("aa", HookType.File));

            Assert.That(supports, Is.True);
        }

        [Test]
        public void SupportsHook_ForClass_ReturnsFalse()
        {
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object, _output);
            var supports = _runner.SupportsHook(new HookTypeRef(typeof(object)));

            Assert.That(supports, Is.False);
        }
        
        [Test]
        public void ExecuteHook_WhenGivenAProcessThatExitsCleanly_DoesntThrow()
        {
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object, _output);
            _runner.ExecuteHook(new HookTypeRef("cmd", HookType.File), "/c exit 0");
        }

        [Test]
        public void ExecuteHook_WhenGivenAProcessThatExitsWithANoneZeroCode_Throws()
        {
            _runner = new CommandLineRunner(_log.Object, _config.Object, _fs.Object, _output);
            var ex = Assert.Throws<HookFailureException>(()=> _runner.ExecuteHook(new HookTypeRef("cmd", HookType.File), "/c exit -300"));

            Assert.That(ex.HookFile, Is.EqualTo("cmd"));
            Assert.That(ex.ExitCode, Is.EqualTo(-300));
        }
    }
}
