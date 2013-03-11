using System.Collections.Generic;
using System.Linq;

namespace deployd_package.Features.MetadataDiscovery
{
    public class InferPackageDetailsFromSingleFoundExecutable : IMetadataDiscoveryHeuristic
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly IPackageDetailsFromAssemblyMapper _fromAssemblyMapper;

        public InferPackageDetailsFromSingleFoundExecutable(System.IO.Abstractions.IFileSystem fs, IPackageDetailsFromAssemblyMapper fromAssemblyMapper)
        {
            _fs = fs;
            _fromAssemblyMapper = fromAssemblyMapper;
        }

        public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata, string discoveryRoot)
        {
            var exesFound = _fs.Directory.GetFiles(discoveryRoot, "*.exe", System.IO.SearchOption.AllDirectories).ToList();
            exesFound.RemoveAll(x => x.Contains(".vshost"));
            
            if (exesFound.Count == 1)
            {
                _fromAssemblyMapper.MapAssemblyInfoToPackage(exesFound[0], discoveredMetadata);
                return;
            }

            if (exesFound.Count > 1)
            {
                return;
            }

            var dllsFound = _fs.Directory.GetFiles(discoveryRoot, "*.dll", System.IO.SearchOption.AllDirectories).ToList();

            if (dllsFound.Count == 1)
            {
                _fromAssemblyMapper.MapAssemblyInfoToPackage(dllsFound[0], discoveredMetadata);
            }
        }
    }
}