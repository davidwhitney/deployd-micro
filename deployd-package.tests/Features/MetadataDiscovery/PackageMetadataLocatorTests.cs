using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _disc = new PackageMetadataLocator();
        }

        [Test]
        public void Metat()
        {
            var metadata = _disc.DiscoverPackageMetadata(_discoveryRoot);
        }
    }
}
