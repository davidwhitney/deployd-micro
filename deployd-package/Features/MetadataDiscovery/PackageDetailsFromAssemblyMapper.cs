using System.Diagnostics;
using log4net;
using NuGet;

namespace deployd_package.Features.MetadataDiscovery
{
    public interface IPackageDetailsFromAssemblyMapper
    {
        void MapAssemblyInfoToPackage(string assembly, PackageMetadata package);
    }

    public class PackageDetailsFromAssemblyMapper : IPackageDetailsFromAssemblyMapper
    {
        private readonly ILog _log;

        public PackageDetailsFromAssemblyMapper(ILog log)
        {
            _log = log;
        }

        public void MapAssemblyInfoToPackage(string assembly, PackageMetadata package)
        {
            _log.Info("Loading assembly metadata... ");

            var versionInfo = FileVersionInfo.GetVersionInfo(assembly);
            
            _log.Info("Product Name: " + versionInfo.ProductName);
            _log.Info("File Description: " + versionInfo.FileDescription);

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