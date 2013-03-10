using System.Collections.Generic;
using NuGet;

namespace deployd_package
{
    public class PackageMetadata
    {
        public string Id { get; set; }
        public SemanticVersion Version { get; set; }
        public string Description { get; set; }
        public List<string> Authors { get; set; }

        public string PackageFilename
        {
            get { return string.Format("{0}-{1}.nupkg", Id, Version); }
        }

        public PackageMetadata()
        {
            Authors = new List<string>();
        }
    }
}