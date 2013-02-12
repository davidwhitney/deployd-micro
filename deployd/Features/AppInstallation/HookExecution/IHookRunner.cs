using deployd.Features.AppInstallation.Hooks;

namespace deployd.Features.AppInstallation.HookExecution
{
    public interface IHookRunner
    {
        void ExecuteHook(Hook hook);
        bool SupportsHook(Hook hook);
    }
}