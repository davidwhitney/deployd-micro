using System.Collections.Generic;
using NuGet;

namespace deployd_package.Features.MetadataDiscovery
{
    public class PackageMetadataLocator
    {
        public PackageMetadataLocator()
        {
            
        }

        public static PackageMetadata DiscoverPackageMetadata(string discoveryRoot)
        {
            return new PackageMetadata
                {
                    Id = "tempid",
                    Version = new SemanticVersion(1, 0, 0, 0),
                    Description = "desc",
                    Authors = new List<string> {"temp-author"}
                };
        }
    }
}