using System.Collections.Generic;

namespace deployd_package.Features.MetadataDiscovery
{
    public class PackageMetadataLocator
    {
        private readonly IEnumerable<IMetadataDiscoveryHeuristic> _discoveryHeuristics;

        public PackageMetadataLocator(IEnumerable<IMetadataDiscoveryHeuristic> discoveryHeuristics)
        {
            _discoveryHeuristics = discoveryHeuristics;
        }

        public PackageMetadata DiscoverPackageMetadata(string discoveryRoot)
        {
            var packageMetadata = new PackageMetadata();

            foreach (var heuristic in _discoveryHeuristics)
            {
                heuristic.DiscoverMetadataProperties(packageMetadata);
            }

            return packageMetadata;
        }
    }
}