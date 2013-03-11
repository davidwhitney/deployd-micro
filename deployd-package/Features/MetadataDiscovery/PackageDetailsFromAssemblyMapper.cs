using System.Diagnostics;
using NuGet;

namespace deployd_package.Features.MetadataDiscovery
{
    public interface IPackageDetailsFromAssemblyMapper
    {
        void MapAssemblyInfoToPackage(string assembly, PackageMetadata package);
    }

    public class PackageDetailsFromAssemblyMapper : IPackageDetailsFromAssemblyMapper
    {
        public void MapAssemblyInfoToPackage(string assembly, PackageMetadata package)
        {
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly);

            package.Id = versionInfo.ProductName;
            package.Description = versionInfo.FileDescription;

            package.Version = new SemanticVersion(versionInfo.FileMajorPart, versionInfo.FileMinorPart,
                                                  versionInfo.FileBuildPart, versionInfo.FilePrivatePart);

            if (!string.IsNullOrWhiteSpace(versionInfo.CompanyName))
            {
                package.Authors.Clear();
                package.Authors.Add(versionInfo.CompanyName);
            }
        }
    }
}