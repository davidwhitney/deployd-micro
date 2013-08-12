using NuGet;
using deployd.Extensibility.Configuration;

namespace deployd.Features.AppLocating
{
    public interface IAppInstallationLocator<TPackageType> : IAppInstallationLocator
    {
        PackageLocation<TPackageType> CanFindPackage(string appName, SemanticVersion version = null);
    }

    public interface IAppInstallationLocator
    {
        PackageLocation<object> CanFindPackageAsObject(string appName, SemanticVersion version = null);
        bool SupportsPathType();
    }
}
