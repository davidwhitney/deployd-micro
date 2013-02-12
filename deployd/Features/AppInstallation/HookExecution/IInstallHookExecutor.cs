namespace deployd.Features.AppInstallation.HookExecution
{
    public interface IInstallHookExecutor
    {
        void ExecuteFirstInstall();
        void ExecutePreInstall();
        void ExecutePostInstall();
    }
}