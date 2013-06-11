using System.Collections.Generic;
using NuGet;

namespace deployd.Features.AppLocating
{
    public interface IListLatestVersionsOfPackagesQuery
    {
        IEnumerable<IPackage> GetLatestVersions(string repoLocation);
    }
}