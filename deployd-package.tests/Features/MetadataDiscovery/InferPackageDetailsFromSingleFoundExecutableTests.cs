using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NUnit.Framework;
using deployd_package.Features.MetadataDiscovery;

namespace deployd_package.tests.Features.MetadataDiscovery
{
    [TestFixture]
    public class InferPackageDetailsFromSingleFoundExecutableTests
    {
        private Mock<IPackageDetailsFromAssemblyMapper> _mapper;
        private string _rootDir;
        

        [SetUp]
        public void SetUp()
        {
            _rootDir = "c:\\package-source-dir";

            _mapper = new Mock<IPackageDetailsFromAssemblyMapper>();
            _mapper.Setup(x => x.MapAssemblyInfoToPackage(It.IsAny<string>(), It.IsAny<PackageMetadata>()))
                .Callback((string s, PackageMetadata md)=> md.Description = "Updated");

        }

        [Test]
        public void PackageMetadata_WhenOneSingleExeIsFound_CallsMapperToMapFromThatExe()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {Path.Combine(_rootDir, "one.exe"), new MockFileData(new byte[0])}
            });

            var ipdfsfe = new InferPackageDetailsFromSingleFoundExecutable(fileSystem, _mapper.Object);
            var meta = new PackageMetadata();

            ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify();
            Assert.That(meta.Description, Is.EqualTo("Updated"));
        }

        [Test]
        public void PackageMetadata_WhenMoreThanOneExeIsFound_MapperNotCalled()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {Path.Combine(_rootDir, "one.exe"), new MockFileData(new byte[0])},
                {Path.Combine(_rootDir, "two.exe"), new MockFileData(new byte[0])},
            });

            var ipdfsfe = new InferPackageDetailsFromSingleFoundExecutable(fileSystem, _mapper.Object);
            var meta = new PackageMetadata();

            ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify(x => x.MapAssemblyInfoToPackage(It.IsAny<string>(), It.IsAny<PackageMetadata>()), Times.Never());
        }

        [Test]
        public void PackageMetadata_WhenNoExesFoundButSingleDll_CallsMapperToMapFromThatDll()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {Path.Combine(_rootDir, "file.dll"), new MockFileData(new byte[0])}
            });

            var ipdfsfe = new InferPackageDetailsFromSingleFoundExecutable(fileSystem, _mapper.Object);
            var meta = new PackageMetadata();

            ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify(x => x.MapAssemblyInfoToPackage(_rootDir + "\\file.dll", It.IsAny<PackageMetadata>()));
        }

        [Test]
        public void PackageMetadata_WhenMultipleExesFound_DllsArentCheckedAsTheyAreLessImportant()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                {Path.Combine(_rootDir, "one.exe"), new MockFileData(new byte[0])},
                {Path.Combine(_rootDir, "two.exe"), new MockFileData(new byte[0])},
                {Path.Combine(_rootDir, "file.dll"), new MockFileData(new byte[0])},
            });

            var ipdfsfe = new InferPackageDetailsFromSingleFoundExecutable(fileSystem, _mapper.Object);
            var meta = new PackageMetadata();

            ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify(x => x.MapAssemblyInfoToPackage(_rootDir + "\\file.dll", It.IsAny<PackageMetadata>()), Times.Never());
        }
    }
}
