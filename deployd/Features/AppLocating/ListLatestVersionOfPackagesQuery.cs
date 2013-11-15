using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet;
using log4net;

namespace deployd.Features.AppLocating
{
    public class ListLatestVersionOfPackagesQuery : IListLatestVersionsOfPackagesQuery
    {
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        private readonly ILog _log;
        private readonly TextWriter _output;

        public ListLatestVersionOfPackagesQuery(IPackageRepositoryFactory packageRepositoryFactory, ILog log, TextWriter output)
        {
            _packageRepositoryFactory = packageRepositoryFactory;
            _log = log;
            _output = output;
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
                _output.WriteLine("No packages could be found at {0}. Is the path or url correct?", repoLocation);
                return null;
            }
        }
    }
}