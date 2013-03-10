using NUnit.Framework;
using NuGet;
using deployd_package.Features.MetadataDiscovery;

namespace deployd_package.tests.Features.MetadataDiscovery
{
    [TestFixture]
    public class DefaultMetadataDiscoveryHeuristicTests
    {
        private DefaultMetadataDiscoveryHeuristic _disc;

        [SetUp]
        public void SetUp()
        {
            _disc = new DefaultMetadataDiscoveryHeuristic();
        }

        [Test]
        public void DiscoverPackageMetadata_MetadataGeneratedHasSensibleDefaults()
        {
            var meta = new PackageMetadata();

            _disc.DiscoverMetadataProperties(meta, string.Empty);

            Assert.That(meta.Authors[0], Is.EqualTo("Author"));
            Assert.That(meta.Description, Is.EqualTo("Package description"));
            Assert.That(meta.Id, Is.EqualTo("Package"));
            Assert.That(meta.Version, Is.EqualTo(new SemanticVersion(0, 0, 0, 0)));
            Assert.That(meta.PackageFilename, Is.EqualTo("Package-0.0.0.0.nupkg"));
        }
    }
}