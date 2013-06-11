using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using log4net;

namespace deployd.Features.AppLocating
{
    public class ListLatestVersionOfPackagesQuery : IListLatestVersionsOfPackagesQuery
    {
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        private readonly ILog _log;

        public ListLatestVersionOfPackagesQuery(IPackageRepositoryFactory packageRepositoryFactory, ILog log)
        {
            _packageRepositoryFactory = packageRepositoryFactory;
            _log = log;
        }

        public IEnumerable<IPackage> GetLatestVersions(string repoLocation)
        {
            var repo = _packageRepositoryFactory.CreateRepository(repoLocation);

            try
            {
                var all = repo.GetPackages()
                              .Where(x =>x.IsLatestVersion)
                              .ToList()
                              .OrderBy(p => p.Id);
                return all;
            }
            catch (Exception ex)
            {
                _log.Error("Could not get packages", ex);
                return null;
            }
        }
    }
}