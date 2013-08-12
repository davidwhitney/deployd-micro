using System;
using System.Linq;
using NuGet;
using log4net;

namespace deployd.Features.AppLocating
{
    public class GetNuGetPackageByNameAndVersionQuery : IGetNuGetPackageByNameAndVersionQuery
    {
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        private readonly ILog _log;

        public GetNuGetPackageByNameAndVersionQuery(IPackageRepositoryFactory packageRepositoryFactory, ILog log)
        {
            _packageRepositoryFactory = packageRepositoryFactory;
            _log = log;
        }

        public IPackage GetPackage(string appName, SemanticVersion version, string repoLocation)
        {
            var repo = _packageRepositoryFactory.CreateRepository(repoLocation);

            try
            {
                return repo.GetPackages()
                    .ToList()
                    .LastOrDefault(x => x.Id == appName && x.Version == version);
            }
            catch (Exception ex)
            {
                _log.Error("Could not get packages", ex);
                return null;
            }
        }
    }
}