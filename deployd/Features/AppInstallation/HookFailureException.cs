using System;

namespace deployd.Features.AppInstallation
{
    public class HookFailureException : Exception
    {
        public string HookFile { get; set; }
        public int ExitCode { get; set; }

        public HookFailureException(string hookFile, int exitCode)
        {
            HookFile = hookFile;
            ExitCode = exitCode;
        }

        public override string Message
        {
            get { return string.Format("Hook execution exited with a non-zero return code. Hook was '{0}'", HookFile); }
        }
    }
}