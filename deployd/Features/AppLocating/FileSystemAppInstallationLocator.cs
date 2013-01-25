namespace deployd.Features.AppLocating
{
    public class FileSystemAppInstallationLocator<TPackageType> : IAppInstallationLocator<TPackageType>
    {
        public PackageLocation<TPackageType> CanFindPackage(string appName)
        {
            return null;
        }

        public PackageLocation<object> CanFindPackageAsObject(string appName)
        {
            return null;
        }
    }
}