using System.Linq;
using System.Text.RegularExpressions;

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

            foreach (var dll in dllsFound)
            {
                var captures = Regex.Matches(dll, @"\\(.*?)\\bin\\(.*?).dll");
                if (captures.Count == 0 || captures[0].Groups.Count < 3)
                {
                    continue;
                }

                var directoryStub = captures[0].Groups[1].Value;
                var dllName = captures[0].Groups[2].Value;

                if (directoryStub.EndsWith(dllName))
                {
                    _fromAssemblyMapper.MapAssemblyInfoToPackage(dll, discoveredMetadata);
                    break;
                }
            }
        }
    }
}