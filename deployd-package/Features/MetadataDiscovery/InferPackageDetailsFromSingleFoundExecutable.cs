using System.Diagnostics;
using System.Linq;
using NuGet;

namespace deployd_package.Features.MetadataDiscovery
{
    public class InferPackageDetailsFromSingleFoundExecutable : IMetadataDiscoveryHeuristic
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;

        public InferPackageDetailsFromSingleFoundExecutable(System.IO.Abstractions.IFileSystem fs)
        {
            _fs = fs;
        }
        
        public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata, string discoveryRoot)
        {
            var matchingFiles = _fs.Directory.GetFiles(discoveryRoot, "*.exe", System.IO.SearchOption.AllDirectories).ToList();
            matchingFiles.RemoveAll(x => x.Contains(".vshost"));

            if (matchingFiles.Count != 1)
            {
                return;
            }

            var vi = FileVersionInfo.GetVersionInfo(matchingFiles[0]);

            discoveredMetadata.Id = vi.ProductName;
            discoveredMetadata.Version = new SemanticVersion(vi.FileMajorPart, vi.FileMinorPart, vi.FileBuildPart, vi.FilePrivatePart);
            discoveredMetadata.Description = vi.FileDescription;

            if (!string.IsNullOrWhiteSpace(vi.CompanyName))
            {
                discoveredMetadata.Authors.Clear();
                discoveredMetadata.Authors.Add(vi.CompanyName);
            }
        }
    }
}