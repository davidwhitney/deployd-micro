using NuGet;

namespace deployd.Features.AppLocating
{
    public interface IGetLatestNuGetPackageByNameQuery
    {
        IPackage GetLatestVersionOf(string appName, string repoLocation);
    }
}