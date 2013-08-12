using NuGet;

namespace deployd.Features.AppLocating
{
    public interface IGetNuGetPackageByNameAndVersionQuery
    {
        IPackage GetPackage(string appName, SemanticVersion version, string repoLocation);
    }
}