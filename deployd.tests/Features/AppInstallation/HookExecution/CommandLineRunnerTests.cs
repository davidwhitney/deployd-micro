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
        private Mock<ILog> _log;
        private Mock<IInstanceConfiguration> _config;
        private CommandLineRunner _runner;

        public CommandLineRunnerTests()
        {
            _log = new Mock<ILog>();
            _config = new Mock<IInstanceConfiguration>();
            _runner = new CommandLineRunner(_log.Object, _config.Object);
        }

        [Test]
        public void VerifyProcessExitCode_WhenProcessExitsWithNoneZeroCode_Throws()
        {
            const string hookFileName = "internal-use-only.ps1";
            const int exitCode = -1;
            var process = RunProcessThatReturns(exitCode);

            var ex = Assert.Throws<HookFailureException>(() => _runner.VerifyProcessExitCode(hookFileName, process));

            Assert.That(ex.HookFile, Is.EqualTo(hookFileName));
            Assert.That(ex.ExitCode, Is.EqualTo(exitCode));
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
