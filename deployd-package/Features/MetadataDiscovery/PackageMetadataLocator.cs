namespace deployd_package.Features.MetadataDiscovery
{
    public class PackageMetadataLocator
    {
        private readonly MetadataDiscoveryHeuristics _discoveryHeuristics;

        public PackageMetadataLocator(MetadataDiscoveryHeuristics discoveryHeuristics)
        {
            _discoveryHeuristics = discoveryHeuristics;
        }

        public PackageMetadata DiscoverPackageMetadata(string discoveryRoot)
        {
            var packageMetadata = new PackageMetadata();

            foreach (var heuristic in _discoveryHeuristics)
            {
                heuristic.DiscoverMetadataProperties(packageMetadata, discoveryRoot);
            }

            return packageMetadata;
        }
    }
}