using System;
using System.Collections.Generic;
using System.Diagnostics;
using log4net;

namespace deployd.Features.AppInstallation
{
    public class InstallHookExecutor
    {
        private readonly ILog _log;
        private readonly HookFinder _finder;
        private readonly Lazy<Hooks> _hooks;

        public InstallHookExecutor(HookFinder finder, ILog log)
        {
            _log = log;
            _finder = finder;
            _hooks = new Lazy<Hooks>(() => _finder.DiscoverHooks());
        }

        public void ExecuteFirstInstall()
        {
            RunHooks(_hooks.Value.FirstInstall);
        }

        public void ExecutePreInstall()
        {
            RunHooks(_hooks.Value.PreInstall);
        }

        public void ExecutePostInstall()
        {
            RunHooks(_hooks.Value.PostInstall);
        }

        private void RunHooks(IEnumerable<string> hookFiles)
        {
            foreach (var hook in hookFiles)
            {
                _log.Info("Executing package hook: " + hook);
                ExecuteHook(hook);
            }
        }

        private void ExecuteHook(string hook)
        {
            var startInfo = new ProcessStartInfo
                {
                    FileName = hook,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                };

            BuildInterpreterPrefixes(hook, startInfo);

            var process = Process.Start(startInfo);

            using (var outputStream = process.StandardOutput)
            {
                process.WaitForExit();
                _log.Info(outputStream.ReadToEnd());
            }

            if (process.ExitCode != 0)
            {
                throw new Exception("Execution of hook failed. Exit code " + process.ExitCode);
            }
        }

        private static void BuildInterpreterPrefixes(string hook, ProcessStartInfo startInfo)
        {
            if (hook.EndsWith(".ps1"))
            {
                startInfo.FileName = "powershell " + startInfo.FileName;
            }
            else if (hook.EndsWith(".rb"))
            {
                startInfo.FileName = "ruby " + startInfo.FileName;
            }
            else if (hook.EndsWith(".py"))
            {
                startInfo.FileName = "python " + startInfo.FileName;
            }
            else if (hook.EndsWith(".cgi"))
            {
                startInfo.FileName = "perl " + startInfo.FileName;
            }
            else if (hook.EndsWith(".php"))
            {
                startInfo.FileName = "php " + startInfo.FileName;
            }
            else if (hook.EndsWith(".js"))
            {
                startInfo.FileName = "node " + startInfo.FileName;
            }
        }
    }
}