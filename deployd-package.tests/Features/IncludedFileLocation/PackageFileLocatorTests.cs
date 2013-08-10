using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Moq;
using NUnit.Framework;
using deployd_package.Features.IncludedFileLocation;
using log4net;

namespace deployd_package.tests.Features.IncludedFileLocation
{
    [TestFixture]
    public class PackageFileLocatorTests
    {
        private PackageFileLocator _pfl;
        private IFileSystem _fs;
        private string _rootDir;
        private Dictionary<string, string[]> _filesOnDisk;
        private Mock<ILog> _log;

        [SetUp]
        public void SetUp()
        {
            _rootDir = "c:\\package-source-dir";
            _log = new Mock<ILog>();

            _fs = new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    {Path.Combine(_rootDir, "sub-1\\one.txt"), new MockFileData("contents")},
                    {Path.Combine(_rootDir, "sub-1\\two.txt"), new MockFileData("contents")},
                    {Path.Combine(_rootDir, "sub-2\\three.txt"), new MockFileData("contents")},
                    {Path.Combine(_rootDir, "sub-2\\four.txt"), new MockFileData("contents")},
                });

            _pfl = new PackageFileLocator(_fs, _log.Object);
        }

        [Test]
        public void IncludedFiles_ReturnsEntryForEachFileInDirectoryStructure()
        {
            var files = _pfl.IncludedFiles(_rootDir).ToList();

            Assert.That(files[0].Path, Is.EqualTo("\\sub-1\\one.txt"));
            Assert.That(files[1].Path, Is.EqualTo("\\sub-1\\two.txt"));
            Assert.That(files[2].Path, Is.EqualTo("\\sub-2\\three.txt"));
            Assert.That(files[3].Path, Is.EqualTo("\\sub-2\\four.txt"));
        }
    }
}
