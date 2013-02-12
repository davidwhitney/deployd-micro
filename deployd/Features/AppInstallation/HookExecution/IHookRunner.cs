using deployd.Features.AppInstallation.Hooks;

namespace deployd.Features.AppInstallation.HookExecution
{
    public interface IHookRunner
    {
        void ExecuteHook(Hook hook, string arguments = null);
        bool SupportsHook(Hook hook);
    }
}