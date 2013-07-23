using System;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation.Hooks;
using log4net;
using IHook = deployd.Extensibility.Hooks.IHook;

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

        public void ExecuteHook(HookTypeRef hookTypeRef, string arguments = null)
        {
            _log.Info("Executing plugin hook: " + hookTypeRef);
            var hookInstance = Activator.CreateInstance(hookTypeRef.Class) as IHook;
            if (hookInstance != null) hookInstance.Execute(_config);
        }

        public bool SupportsHook(HookTypeRef hookTypeRef)
        {
            return hookTypeRef.Type == HookType.Class;
        }
    }
}