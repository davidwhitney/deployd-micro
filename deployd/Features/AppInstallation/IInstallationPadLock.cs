namespace deployd.Features.AppInstallation
{
    public interface IInstallationPadLock
    {
        void LockAppInstallation();
        void UnlockAppInstallation();
    }
}