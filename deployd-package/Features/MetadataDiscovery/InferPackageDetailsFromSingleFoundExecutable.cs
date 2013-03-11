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
            var matchingFiles = _fs.Directory.GetFiles(discoveryRoot, "*.exe", System.IO.SearchOption.AllDirectories).ToList();
            matchingFiles.RemoveAll(x => x.Contains(".vshost"));

            if (matchingFiles.Count != 1)
            {
                return;
            }

            _fromAssemblyMapper.MapAssemblyInfoToPackage(matchingFiles[0], discoveredMetadata);
        }
    }
}