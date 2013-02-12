using System.Diagnostics;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using deployd.Features.AppInstallation.HookExecution;
using log4net;

namespace deployd.tests.Features.AppInstallation.HookExecution
{
    [TestFixture]
    public class CommandLineRunnerTests
    {
        private readonly Mock<ILog> _log;
        private readonly Mock<IInstanceConfiguration> _config;
        private readonly CommandLineRunner _runner;
        const string HookFileName = "internal-use-only.ps1";

        public CommandLineRunnerTests()
        {
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _runner = new CommandLineRunner(_log.Object, _config.Object);
        }

        [Test]
        public void VerifyProcessExitCode_WhenProcessExitsWithNoneZeroCode_Throws()
        {
            const int exitCode = -1;
            var process = RunProcessThatReturns(exitCode);

            var ex = Assert.Throws<HookFailureException>(() => _runner.VerifyProcessExitCode(HookFileName, process));

            Assert.That(ex.HookFile, Is.EqualTo(HookFileName));
            Assert.That(ex.ExitCode, Is.EqualTo(exitCode));
        }

        [TestCase("ps1", "powershell")]
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

        private static Process RunProcessThatReturns(int exitCode)
        {
            var startInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c exit " + exitCode,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

            var process = Process.Start(startInfo);
            process.WaitForExit();
            return process;
        }
    }
}
