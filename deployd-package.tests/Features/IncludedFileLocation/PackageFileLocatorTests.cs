using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Moq;
using NUnit.Framework;
using deployd_package.Features.IncludedFileLocation;

namespace deployd_package.tests.Features.IncludedFileLocation
{
    [TestFixture]
    public class PackageFileLocatorTests
    {
        private PackageFileLocator _pfl;
        private Mock<IFileSystem> _fs;
        private string _rootDir;
        private Dictionary<string, string[]> _filesOnDisk;

        [SetUp]
        public void SetUp()
        {
            _rootDir = "c:\\package-source-dir";
            _fs = new Mock<IFileSystem>();

            _filesOnDisk = new Dictionary<string, string[]>
                {
                    {_rootDir + "\\sub-1", new[] {_rootDir + "\\sub-1\\one.txt", _rootDir + "\\sub-1\\two.txt"}},
                    {_rootDir + "\\sub-2", new[] {_rootDir + "\\sub-2\\three.txt", _rootDir + "\\sub-2\\four.txt"}}
                };

            _fs.Setup(x => x.Directory.GetDirectories(_rootDir)).Returns(_filesOnDisk.Keys.ToArray());
            _fs.Setup(x => x.Directory.GetFiles(It.IsAny<string>()))
               .Returns((string param) => _filesOnDisk.ContainsKey(param) ? _filesOnDisk[param].ToArray() : new string[0]);
            
            _pfl = new PackageFileLocator(_fs.Object, _rootDir);
        }

        [Test]
        public void IncludedFiles_ReturnsEntryForEachFileInDirectoryStructure()
        {
            var files = _pfl.IncludedFiles().ToList();

            Assert.That(files[0].Path, Is.EqualTo("sub-1\\one.txt"));
            Assert.That(files[1].Path, Is.EqualTo("sub-1\\two.txt"));
            Assert.That(files[2].Path, Is.EqualTo("sub-2\\three.txt"));
            Assert.That(files[3].Path, Is.EqualTo("sub-2\\four.txt"));
        }
    }
}
