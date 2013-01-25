using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            if (hook.EndsWith(".ps1"))
            {
                startInfo.FileName = "powershell " + startInfo.FileName;
            }
            else if (hook.EndsWith(".rb"))
            {
                startInfo.FileName = "ruby " + startInfo.FileName;
            }

            var process = Process.Start(startInfo);
            using (var outputStream = process.StandardOutput)
            {
                process.WaitForExit();
                _log.Info(outputStream.ReadToEnd());
            }


        }
    }
}