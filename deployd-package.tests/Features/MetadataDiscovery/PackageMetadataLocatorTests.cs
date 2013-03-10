using System.Collections.Generic;
using NUnit.Framework;
using NuGet;
using deployd_package.Features.MetadataDiscovery;

namespace deployd_package.tests.Features.MetadataDiscovery
{
    [TestFixture]
    public class PackageMetadataLocatorTests
    {
        private string _discoveryRoot;
        private PackageMetadataLocator _disc;

        [SetUp]
        public void SetUp()
        {
            _discoveryRoot = "";
            _disc = new PackageMetadataLocator();
        }

        [Test]
        public void DiscoverPackageMetadata_ExecutesEmbeddedLocationHeuristics()
        {
            var heuristic = new FakeHeuristic();
            _disc = new PackageMetadataLocator(new List<IMetadataDiscoveryHeuristic> { heuristic });
            
            _disc.DiscoverPackageMetadata(_discoveryRoot);

            Assert.That(heuristic.Executed, Is.True);
        }

        [Test]
        public void DiscoverPackageMetadata_MetadataReturnedIsNotNull()
        {
            var meta = _disc.DiscoverPackageMetadata(_discoveryRoot);
            Assert.That(meta, Is.Not.Null);
        }

        [Test]
        public void DiscoverPackageMetadata_MetadataGeneratedHasSensibleDefaults()
        {
            var meta = _disc.DiscoverPackageMetadata(_discoveryRoot);

            Assert.That(meta.Authors[0], Is.EqualTo("Author"));
            Assert.That(meta.Description, Is.EqualTo("Package description"));
            Assert.That(meta.Id, Is.EqualTo("Package"));
            Assert.That(meta.Version, Is.EqualTo(new SemanticVersion(0,0,0,0)));
            Assert.That(meta.PackageFilename, Is.EqualTo("Package-0.0.0.0.nupkg"));

        }

        private class FakeHeuristic : IMetadataDiscoveryHeuristic
        {
            public bool Executed { get; set; }
            public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata)
            {
                Executed = true;
            }
        }
    }
}
