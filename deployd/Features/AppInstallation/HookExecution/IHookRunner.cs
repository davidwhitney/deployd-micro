using deployd.Features.AppInstallation.Hooks;

namespace deployd.Features.AppInstallation.HookExecution
{
    public interface IHookRunner
    {
        void ExecuteHook(HookTypeRef hookTypeRef, string arguments = null);
        bool SupportsHook(HookTypeRef hookTypeRef);
    }
}