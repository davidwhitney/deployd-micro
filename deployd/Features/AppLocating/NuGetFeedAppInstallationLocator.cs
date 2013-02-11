using System;
using System.Linq;
using NuGet;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.AppLocating
{
    public class NuGetFeedAppInstallationLocator : IAppInstallationLocator<IPackage>
    {
        private readonly DeploydConfiguration _clientConfig;
        private readonly IFileSystem _fs;
        private readonly IGetLatestNuGetPackageByNameQuery _query;

        public bool IsHttp { get { return _clientConfig.PackageSource.StartsWith("http"); } }
        public string PackageLocation { get { return IsHttp ? _clientConfig.PackageSource : _clientConfig.PackageSource.ToAbsolutePath(); } }

        public NuGetFeedAppInstallationLocator(DeploydConfiguration clientConfig, IFileSystem fs, IGetLatestNuGetPackageByNameQuery query)
        {
            _clientConfig = clientConfig;
            _fs = fs;
            _query = query;
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