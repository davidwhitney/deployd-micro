using System;
using System.Linq;
using NuGet;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.ClientConfiguration;
using log4net;
using deployd.Infrastructure;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.AppLocating
{
    public class NuGetFeedAppInstallationLocator : IAppInstallationLocator<IPackage>
    {
        private readonly ILog _log;
        private readonly IFileSystem _fs;
        private readonly DeploydConfiguration _clientConfig;
        private readonly IPackageRepository _packageRepository;

        public NuGetFeedAppInstallationLocator(ILog log, System.IO.Abstractions.IFileSystem fs, DeploydConfiguration clientConfig, IPackageRepositoryFactory packageRepositoryFactory)
        {
            _log = log;
            _fs = fs;
            _clientConfig = clientConfig;

            var repoLocation = clientConfig.PackageSource;
            if (!repoLocation.StartsWith("http"))
            {
                repoLocation = repoLocation.ToAbsolutePath();
            }

            _packageRepository = packageRepositoryFactory.CreateRepository(repoLocation);
        }
        
        public bool SupportsPathType()
        {
            return _clientConfig.PackageSource.StartsWith("http") 
                    || _fs.Directory.Exists(_clientConfig.PackageSource.ToAbsolutePath());
        }

        public PackageLocation<IPackage> CanFindPackage(string appName)
        {
            try
            {
                var all = _packageRepository.GetPackages()
                                            .Where(x => x.Id == appName && x.IsLatestVersion)
                                            .ToList();
                all.Reverse();

                var latestPackage = all.FirstOrDefault();

                if (latestPackage != null)
                {
                    return new PackageLocation<IPackage>
                        {
                            PackageDetails = latestPackage,
                            PackageVersion = latestPackage.Version.Version.ToString()
                        };
                }
            }
            catch (Exception ex)
            {
                _log.Error("Could not get packages", ex);
            }

            return null;
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