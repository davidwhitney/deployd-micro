using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.Features.AppInstallation.HookExecution
{
    public class CommandLineRunner : IHookRunner
    {
        private readonly ILog _log;
        private readonly IInstanceConfiguration _config;
        private readonly IFileSystem _fs;

        private static readonly Dictionary<string, string> ExecutableMap = new Dictionary<string, string>
            {
                //{"ps1", "powershell"},
                {"rb", "ruby"},
                {"py", "python"},
                {"cgi", "perl"},
                {"php", "php"},
                {"js", "node"},
            };

        public CommandLineRunner(ILog log, IInstanceConfiguration config, IFileSystem fs)
        {
            _log = log;
            _config = config;
            _fs = fs;
        }

        public bool SupportsHook(HookTypeRef hookTypeRef)
        {
            return hookTypeRef.Type == HookType.File
                && ExecutableMap.ContainsKey(_fs.Path.GetExtension(hookTypeRef.FileName).Substring(1));
        }

        public void ExecuteHook(HookTypeRef hookTypeRef, string arguments = null)
        {
            _log.Info("Executing package hookFileName: " + hookTypeRef.FileName);

            var hookFilename = hookTypeRef.FileName;
            var startInfo = new ProcessStartInfo
                {
                    FileName = hookFilename,
                    Arguments = arguments ?? string.Empty,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

            CopyVariablesToEnvironment(startInfo);
            PrefixCommonScriptRuntimes(hookFilename, startInfo);
            StartProcess(hookFilename, startInfo);
        }

        private void StartProcess(string hookFileName, ProcessStartInfo startInfo)
        {
            var process = Process.Start(startInfo);

            using (var outputStream = process.StandardOutput)
            {
                process.WaitForExit();
                _log.Info(outputStream.ReadToEnd());
            }

            if (process.ExitCode != 0)
            {
                throw new HookFailureException(hookFileName, process.ExitCode);
            }
        }

        public void PrefixCommonScriptRuntimes(string hookFileName, ProcessStartInfo startInfo)
        {
            foreach (var extension in ExecutableMap.Where(ext => hookFileName.EndsWith("." + ext.Key)))
            {
                startInfo.FileName = extension.Value + " " + startInfo.FileName;
            }
        }

        public void CopyVariablesToEnvironment(ProcessStartInfo startInfo)
        {
            var envrs = _config.ApplicationMap.GetType().GetProperties()
                              .Select(fi => new {Field = fi.Name, Value = fi.GetValue(_config.ApplicationMap,null)})
                              .ToList();

            foreach (var variable in envrs.Where(variable => variable.Value != null))
            {
                startInfo.EnvironmentVariables.Add("Deployd." + variable.Field, variable.Value.ToString());
            }
        }
    }
}