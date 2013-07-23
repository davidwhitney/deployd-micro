using System;
using System.Collections.Generic;
using System.Linq;
using deployd.Features.AppInstallation.Hooks;

namespace deployd.Features.AppInstallation.HookExecution
{
    public class InstallHookExecutor : IInstallHookExecutor
    {
        private readonly IEnumerable<IHookRunner> _hookRunners;
        private readonly IHookFinder _finder;
        private readonly Lazy<Hooks.Hooks> _hooks;
        
        public InstallHookExecutor(IHookFinder finder, IEnumerable<IHookRunner> hookRunners)
        {
            _hookRunners = hookRunners;
            _finder = finder;
            _hooks = new Lazy<Hooks.Hooks>(() => _finder.DiscoverHooks());
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

        public void ExecuteFirstPostInstall()
        {
            RunHooks(_hooks.Value.PostFirstInstall);
        }

        private void RunHooks(IEnumerable<HookTypeRef> hookFiles)
        {
            foreach (var hook in hookFiles)
            {
                _hookRunners.First(x => x.SupportsHook(hook)).ExecuteHook(hook);
            }
        }
    }
}