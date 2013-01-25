using System.Collections.Generic;
using System.Diagnostics;

namespace deployd.Features.AppInstallation
{
    public class InstallHookExecutor
    {
        private readonly HookFinder _finder;

        private Hooks _hooks;
        private Hooks Hooks
        {
            get { return _hooks ?? (_hooks = _finder.DiscoverHooks()); }
        }

        public InstallHookExecutor(HookFinder finder)
        {
            _finder = finder;
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
            var startInfo = new ProcessStartInfo {FileName = hook};

            if (hook.EndsWith(".ps1"))
            {
                startInfo.FileName = "powershell " + startInfo.FileName;
            }
            else if (hook.EndsWith(".rb"))
            {
                startInfo.FileName = "ruby " + startInfo.FileName;
            }

            Process.Start(startInfo);
        }
    }
}