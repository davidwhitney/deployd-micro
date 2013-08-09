using NuGet;

namespace deployd.Features.AppExtraction
{
    public interface IPackageCache
    {
        IPackage CachePackage(IPackage package);
        IPackage GetPackage(string packageId, string version=null);
    }
}