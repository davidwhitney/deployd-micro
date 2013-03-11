using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NuGet;
using deployd_package.Features.MetadataDiscovery;
using deployd_package.Features.PackageBuilding;

namespace deployd_package.tests.Features.PackageBuilding
{
    [TestFixture]
    public class PackageConstructorTests
    {
        private PackageConstructor _pc;
        private List<IPackageFile> _files;

        [SetUp]
        public void SetUp()
        {
            _files = new List<IPackageFile>();
            _pc = new PackageConstructor();
        }

        [Test]
        public void AuthorsInMetaData_MapsToPackage()
        {
            var metaData = new PackageMetadata { Authors = new List<string> {"SomeAuthor"} };
            
            var packageBuilt = _pc.BuildPackage(_files, metaData);

            Assert.That(packageBuilt.Authors.First(), Is.EqualTo("SomeAuthor"));
        }

        [Test]
        public void IdInMetaData_MapsToPackage()
        {
            var metaData = new PackageMetadata { Id = "something" };
            
            var packageBuilt = _pc.BuildPackage(_files, metaData);

            Assert.That(packageBuilt.Id, Is.EqualTo("something"));
        }

        [Test]
        public void VersionInMetaData_MapsToPackage()
        {
            var metaData = new PackageMetadata { Version = new SemanticVersion(9,8,7,6) };
            
            var packageBuilt = _pc.BuildPackage(_files, metaData);

            Assert.That(packageBuilt.Version, Is.EqualTo(new SemanticVersion(9, 8, 7, 6)));
        }

        [Test]
        public void DescriptionInMetaData_MapsToPackage()
        {
            var metaData = new PackageMetadata { Description = "my huge package" };
            
            var packageBuilt = _pc.BuildPackage(_files, metaData);

            Assert.That(packageBuilt.Description, Is.EqualTo("my huge package"));
        }

        [Test]
        public void FileInList_AddedToPackage()
        {
            var metaData = new PackageMetadata();
            var file = new PhysicalPackageFile();
            _files.Add(file);
            
            var packageBuilt = _pc.BuildPackage(_files, metaData);

            Assert.That(packageBuilt.Files[0], Is.EqualTo(file));
        }
    }
}
