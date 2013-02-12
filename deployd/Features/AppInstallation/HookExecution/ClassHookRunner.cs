using System;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation.Hooks;
using log4net;

namespace deployd.Features.AppInstallation.HookExecution
{
    public class ClassHookRunner : IHookRunner
    {
        private readonly ILog _log;
        private readonly IInstanceConfiguration _config;

        public ClassHookRunner(ILog log, IInstanceConfiguration config)
        {
            _log = log;
            _config = config;
        }

        public void ExecuteHook(Hook hook, string arguments = null)
        {
            _log.Info("Executing plugin hook: " + hook);
            var classs = (IHook)Activator.CreateInstance(hook.Class);
            classs.Execute(_config);
        }

        public bool SupportsHook(Hook hook)
        {
            return hook.Type == HookType.Class;
        }
    }
}