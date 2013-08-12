using NuGet;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.AppLocating
{
    public class NuGetFeedAppInstallationLocator : IAppInstallationLocator<IPackage>
    {
        private readonly IFileSystem _fs;
        private readonly IGetLatestNuGetPackageByNameQuery _latestPackageQuery;
        private readonly IGetNuGetPackageByNameAndVersionQuery _packageByNameAndVersionQuery;
        private readonly string _packageLocation;

        public bool IsHttp { get { return _packageLocation.StartsWith("http"); } }
        public string PackageLocation { get { return _packageLocation; } }

        public NuGetFeedAppInstallationLocator(DeploydConfiguration clientConfig, IFileSystem fs, IGetLatestNuGetPackageByNameQuery latestPackageQuery,
            IGetNuGetPackageByNameAndVersionQuery packageByNameAndVersionQuery)
            :this(clientConfig.PackageSource, fs, latestPackageQuery, packageByNameAndVersionQuery)
        {
        }

        public NuGetFeedAppInstallationLocator(string packageSource, IFileSystem fs,
                                               IGetLatestNuGetPackageByNameQuery latestPackageQuery,
                                                IGetNuGetPackageByNameAndVersionQuery packageByNameAndVersionQuery)
        {
            _fs = fs;
            _latestPackageQuery = latestPackageQuery;
            _packageByNameAndVersionQuery = packageByNameAndVersionQuery;

            if (packageSource.StartsWith("http"))
            {
                _packageLocation = packageSource;
            }
            else
            {
                _packageLocation = packageSource.ToAbsolutePath();
            }
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