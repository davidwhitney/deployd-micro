namespace deployd.Features.AppLocating
{
    public interface IAppInstallationLocator<TPackageType> : IAppInstallationLocator
    {
        PackageLocation<TPackageType> CanFindPackage(string appName);
    }

    public interface IAppInstallationLocator
    {
        PackageLocation<object> CanFindPackageAsObject(string appName);
    }
}
