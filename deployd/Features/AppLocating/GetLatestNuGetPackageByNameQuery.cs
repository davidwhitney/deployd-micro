using System;
using System.Linq;
using NuGet;
using log4net;

namespace deployd.Features.AppLocating
{
    public class GetLatestNuGetPackageByNameQuery : IGetLatestNuGetPackageByNameQuery
    {
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        private readonly ILog _log;

        public GetLatestNuGetPackageByNameQuery(IPackageRepositoryFactory packageRepositoryFactory, ILog log)
        {
            _packageRepositoryFactory = packageRepositoryFactory;
            _log = log;
        }

        public IPackage GetLatestVersionOf(string appName, string repoLocation)
        {
            var repo = _packageRepositoryFactory.CreateRepository(repoLocation);

            try
            {
                var all = repo.GetPackages()
                              .Where(x => x.Id == appName/* && x.IsLatestVersion*/)
                              .ToList();
                all.Reverse();
                return all.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _log.Error("Could not get packages", ex);
                return null;
            }
        }
    }
}