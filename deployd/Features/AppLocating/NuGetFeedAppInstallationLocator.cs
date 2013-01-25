using System;
using System.Linq;
using NuGet;
using deployd.Features.ClientConfiguration;
using log4net;
using deployd.Infrastructure;

namespace deployd.Features.AppLocating
{
    public class NuGetFeedAppInstallationLocator : IAppInstallationLocator<IPackage>
    {
        private readonly ILog _log;
        private readonly IPackageRepository _packageRepository;

        public NuGetFeedAppInstallationLocator(ILog log, Configuration clientConfig, IPackageRepositoryFactory packageRepositoryFactory)
        {
            _log = log;
            
            var repoLocation = clientConfig.PackageSource;
            if (!repoLocation.StartsWith("http"))
            {
                repoLocation = repoLocation.ToAbsolutePath();
            }

            _packageRepository = packageRepositoryFactory.CreateRepository(repoLocation);
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
                    return new PackageLocation<IPackage> { PackageDetails = latestPackage };
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
            return inner == null ? null : new PackageLocation<object>{ PackageDetails = inner.PackageDetails };
        }
    }
}