using System;
using NuGet;

namespace deployd_package.Features.MetadataDiscovery
{
    public class DefaultMetadataDiscoveryHeuristic : IMetadataDiscoveryHeuristic
    {
        public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata, string discoveryRoot)
        {
            discoveredMetadata.Id = "Package";
            discoveredMetadata.Version = new SemanticVersion(0, 0, 0, 0);
            discoveredMetadata.Description = "Package description";
            discoveredMetadata.Authors.Add("Author");
        }
    }
}