using System.Collections.Generic;
using NUnit.Framework;
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
            _disc = new PackageMetadataLocator(new List<IMetadataDiscoveryHeuristic>());
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

        private class FakeHeuristic : IMetadataDiscoveryHeuristic
        {
            public bool Executed { get; private set; }
            public void DiscoverMetadataProperties(PackageMetadata discoveredMetadata)
            {
                Executed = true;
            }
        }
    }
}