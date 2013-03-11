using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NUnit.Framework;
using deployd_package.Features.MetadataDiscovery;

namespace deployd_package.tests.Features.MetadataDiscovery
{
    [TestFixture]
    public class InferPackageDetailsFromSingleFoundExecutableTests
    {
        private Mock<IPackageDetailsFromAssemblyMapper> _mapper;
        private Mock<IFileSystem> _fs;
        private InferPackageDetailsFromSingleFoundExecutable _ipdfsfe;
        private string _rootDir;
        private List<string> _exeFilesOnDisk;
        private List<string> _dllFilesOnDisk;

        [SetUp]
        public void SetUp()
        {
            _rootDir = "c:\\package-source-dir";
            _fs = new Mock<IFileSystem>();

            _exeFilesOnDisk = new List<string>();
            _dllFilesOnDisk = new List<string>();
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), "*.exe", SearchOption.AllDirectories)).Returns(_exeFilesOnDisk.ToArray);
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), "*.dll", SearchOption.AllDirectories)).Returns(_dllFilesOnDisk.ToArray);
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>(), "*.dll", SearchOption.AllDirectories)).Returns(_dllFilesOnDisk.ToArray);

            _mapper = new Mock<IPackageDetailsFromAssemblyMapper>();
            _mapper.Setup(x => x.MapAssemblyInfoToPackage(It.IsAny<string>(), It.IsAny<PackageMetadata>()))
                .Callback((string s, PackageMetadata md)=> md.Description = "Updated");

            _ipdfsfe = new InferPackageDetailsFromSingleFoundExecutable(_fs.Object, _mapper.Object);
        }

        [Test]
        public void PackageMetadata_WhenOneSingleExeIsFound_CallsMapperToMapFromThatExe()
        {
            var meta = new PackageMetadata();
            _exeFilesOnDisk.Add(_rootDir + "\\one.exe");

            _ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify();
            Assert.That(meta.Description, Is.EqualTo("Updated"));
        }

        [Test]
        public void PackageMetadata_WhenMoreThanOneExeIsFound_MapperNotCalled()
        {
            var meta = new PackageMetadata();
            _exeFilesOnDisk.Add(_rootDir + "\\one.exe");
            _exeFilesOnDisk.Add(_rootDir + "\\two.exe");

            _ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify(x => x.MapAssemblyInfoToPackage(It.IsAny<string>(), It.IsAny<PackageMetadata>()), Times.Never());
        }

        [Test]
        public void PackageMetadata_WhenNoExesFoundButSingleDll_CallsMapperToMapFromThatDll()
        {
            var meta = new PackageMetadata();
            _dllFilesOnDisk.Add(_rootDir + "\\file.dll");

            _ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify(x => x.MapAssemblyInfoToPackage(_rootDir + "\\file.dll", It.IsAny<PackageMetadata>()));
        }

        [Test]
        public void PackageMetadata_WhenMultipleExesFound_DllsArentCheckedAsTheyAreLessImportant()
        {
            var meta = new PackageMetadata();
            _exeFilesOnDisk.Add(_rootDir + "\\one.exe");
            _exeFilesOnDisk.Add(_rootDir + "\\two.exe");
            _dllFilesOnDisk.Add(_rootDir + "\\file.dll");

            _ipdfsfe.DiscoverMetadataProperties(meta, _rootDir);

            _mapper.Verify(x => x.MapAssemblyInfoToPackage(_rootDir + "\\file.dll", It.IsAny<PackageMetadata>()), Times.Never());
        }
    }
}
