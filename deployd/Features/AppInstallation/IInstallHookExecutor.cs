namespace deployd.Features.AppInstallation
{
    public interface IInstallHookExecutor
    {
        void ExecuteFirstInstall();
        void ExecutePreInstall();
        void ExecutePostInstall();
    }
}