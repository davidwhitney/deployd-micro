using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using Moq;
using NUnit.Framework;
using deployd_package.Features.MetadataDiscovery;

namespace deployd_package.tests.Features.MetadataDiscovery
{
    [TestFixture]
    public class DiscoverPackageConfigurationMetadataFileTests
    {
        private Mock<IFileSystem> _fs;
        private DiscoverPackageConfigurationMetadataFile _heuristic;
        private List<string> _packageManifests;
        private string _discoveryRoot;
        private const string SettingsFilename = "settings.deployd-package";

        [SetUp]
        public void SetUp()
        {
            _discoveryRoot = "c:\\root";
            _fs = new Mock<IFileSystem>();
            _packageManifests = new List<string>();
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), "*.deployd-package", SearchOption.AllDirectories)).Returns(_packageManifests.ToArray);
            _heuristic = new DiscoverPackageConfigurationMetadataFile(_fs.Object);
        }

        [Test]
        public void MoreThanOnePackageManifestPresent_ThrowsException()
        {
            var meta = new PackageMetadata();
            _packageManifests.Add(SettingsFilename);
            _packageManifests.Add("settings2.deployd-package");

            var ex = Assert.Throws<InvalidOperationException>(() => _heuristic.DiscoverMetadataProperties(meta, _discoveryRoot));

            Assert.That(ex.Message, Is.EqualTo("More than one packing convention file found in source directory."));
        }

        [Test]
        public void OnePackageManifestPresent_LoadsPackageSettingsFile()
        {
            var meta = new PackageMetadata();
            _packageManifests.Add(SettingsFilename);
            _fs.Setup(x => x.File.ReadAllText(SettingsFilename)).Returns(string.Empty);

            _heuristic.DiscoverMetadataProperties(meta, _discoveryRoot);

            _fs.Verify(x => x.File.ReadAllText(SettingsFilename), Times.Once());
        }
    }
}
