using NuGet;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.AppLocating
{
    public class NuGetFeedAppInstallationLocator : IAppInstallationLocator<IPackage>
    {
        private readonly IFileSystem _fs;
        private readonly IGetLatestNuGetPackageByNameQuery _query;
        private readonly string _packageLocation;

        public bool IsHttp { get { return _packageLocation.StartsWith("http"); } }
        public string PackageLocation { get { return _packageLocation; } }

        public NuGetFeedAppInstallationLocator(DeploydConfiguration clientConfig, IFileSystem fs, IGetLatestNuGetPackageByNameQuery query)
            :this(clientConfig.PackageSource, fs, query)
        {
        }

        public NuGetFeedAppInstallationLocator(string packageSource, IFileSystem fs,
                                               IGetLatestNuGetPackageByNameQuery query)
        {
            _fs = fs;
            _query = query;

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

        public PackageLocation<IPackage> CanFindPackage(string appName)
        {
            var latestPackage = _query.GetLatestVersionOf(appName, PackageLocation);

            return latestPackage == null
                       ? null
                       : new PackageLocation<IPackage>
                           {
                               PackageDetails = latestPackage,
                               PackageVersion = latestPackage.Version.Version.ToString()
                           };
        }

        public PackageLocation<object> CanFindPackageAsObject(string appName)
        {
            var inner = CanFindPackage(appName);
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