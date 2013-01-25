namespace deployd.Features.AppLocating
{
    public interface IAppInstallationLocator
    {
        PackageLocation CanFindPackage(string appName);
    }
}
