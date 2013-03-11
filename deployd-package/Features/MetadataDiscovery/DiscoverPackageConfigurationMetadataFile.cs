using System;
using System.Linq;

namespace deployd_package.Features.MetadataDiscovery
{
    public class DiscoverPackageConfigurationMetadataFile : IMetadataDiscoveryHeuristic
    {       
        private readonly System.IO.Abstractions.IFileSystem _fs;

        public DiscoverPackageConfigurationMetadataFile(System.IO.Abstractions.IFileSystem fs)
        {
            _fs = fs;
        }

        public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata, string discoveryRoot)
        {
            var matchingFiles = _fs.Directory.GetFiles(discoveryRoot, "*.deployd-package", System.IO.SearchOption.AllDirectories).ToList();

            if (matchingFiles.Count == 0)
            {
                return;
            }

            if (matchingFiles.Count > 1)
            {
                throw new InvalidOperationException("More than one packing convention file found in source directory.");
            }

            var conventions = _fs.File.ReadAllText(matchingFiles[0]);
        }
    }
}