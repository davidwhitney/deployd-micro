using NuGet;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.AppLocating
{
    public class NuGetFeedAppInstallationLocator : IAppInstallationLocator<IPackage>
    {
        private readonly IPackageSourceConfiguration _packageSourceConfiguration;
        private readonly IFileSystem _fs;
        private readonly IGetLatestNuGetPackageByNameQuery _latestPackageQuery;
        private readonly IGetNuGetPackageByNameAndVersionQuery _packageByNameAndVersionQuery;

        public bool IsHttp { get { return _packageSourceConfiguration.PackageSource.StartsWith("http"); } }
        public string PackageLocation { get { return _packageSourceConfiguration.PackageSource; } }

        public NuGetFeedAppInstallationLocator(IPackageSourceConfiguration packageSourceConfiguration, IFileSystem fs,
                                               IGetLatestNuGetPackageByNameQuery latestPackageQuery,
                                                IGetNuGetPackageByNameAndVersionQuery packageByNameAndVersionQuery)
        {
            _packageSourceConfiguration = packageSourceConfiguration;
            _fs = fs;
            _latestPackageQuery = latestPackageQuery;
            _packageByNameAndVersionQuery = packageByNameAndVersionQuery;
        }
        
        public bool SupportsPathType()
        {
            return IsHttp || _fs.Directory.Exists(PackageLocation);
        }

        public PackageLocation<IPackage> CanFindPackage(string appName, SemanticVersion version=null)
        {
            IPackage package;
            if (version == null)
                package = _latestPackageQuery.GetLatestVersionOf(appName, PackageLocation);
            else
                package = _packageByNameAndVersionQuery.GetPackage(appName, version, PackageLocation);

            return package == null
                       ? null
                       : new PackageLocation<IPackage>
                       {
                           PackageDetails = package,
                           PackageVersion = package.Version.Version.ToString()
                       };
        }

        public PackageLocation<object> CanFindPackageAsObject(string appName, SemanticVersion version = null)
        {
            var inner = CanFindPackage(appName, version);
            return inner == null
                       ? null
                       : new PackageLocation<object>
                           {
                               PackageDetails = inner.PackageDetails,
                               PackageVersion = inner.PackageVersion
                           };
        }
    }
}