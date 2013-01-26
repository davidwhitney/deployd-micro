using System.Collections.Generic;
using System.Diagnostics;
using log4net;

namespace deployd.Features.AppInstallation
{
    public class InstallHookExecutor
    {
        private readonly HookFinder _finder;
        private readonly ILog _log;

        private Hooks _hooks;
        private Hooks Hooks
        {
            get { return _hooks ?? (_hooks = _finder.DiscoverHooks()); }
        }

        public InstallHookExecutor(HookFinder finder, ILog log)
        {
            _finder = finder;
            _log = log;
        }

        public void ExecuteFirstInstall()
        {
            RunHooks(Hooks.FirstInstall);
        }

        public void ExecutePreInstall()
        {
            RunHooks(Hooks.PreInstall);
        }

        public void ExecutePostInstall()
        {
            RunHooks(Hooks.PostInstall);
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