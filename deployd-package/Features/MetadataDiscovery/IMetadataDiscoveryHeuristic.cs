namespace deployd_package.Features.MetadataDiscovery
{
    public interface IMetadataDiscoveryHeuristic
    {
        void DiscoverMetadataProperties(PackageMetadata discoveredMetadata, string discoveryRoot);
    }
}