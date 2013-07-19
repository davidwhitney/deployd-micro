using NuGet;

namespace deployd.Features.AppExtraction
{
    public interface IPackageCache
    {
        IPackage CachePackage(IPackage package);
    }
}