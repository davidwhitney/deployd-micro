using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Moq;
using NUnit.Framework;
using System.IO.Abstractions;
using deployd.Extensibility.Configuration;

namespace deployd.tests.Features.ClientConfiguration
{
    [TestFixture]
    public class ClientConfigurationManagerTests
    {
        private Mock<IFileSystem> _fs;
        private DeploydConfigurationManager _cfgManager;
        private Mock<IApplicationFolderLocator> _appFolderLocator;

        [SetUp]
        public void SetUp()
        {
            _fs = new Mock<IFileSystem>();
            IMockFileDataAccessor fileData=new MockFileSystem(new Dictionary<string, MockFileData>());
            _fs.SetupGet(x => x.Path).Returns(new MockPath(fileData));
            _appFolderLocator = new Mock<IApplicationFolderLocator>();
            _appFolderLocator.SetupGet(x => x.ApplicationFolder).Returns("");
            _cfgManager = new DeploydConfigurationManager(_fs.Object, _appFolderLocator.Object);
        }

        [Test]
        public void LoadConfig_ConfigFileFound_LoadsConfiguration()
        {
            const string configFileContents = @"
                    { 
                        ""PackageType"" : ""NuGet"", 
                        ""PackageSource"" : ""d:\\test"" ,
                        ""InstallRoot"" : ""Apps""
                    }";

            _fs.Setup(x => x.File.Exists("config.json")).Returns(true);
            _fs.Setup(x => x.File.ReadAllText("config.json")).Returns(configFileContents);

            var configuration = _cfgManager.LoadConfig();

            Assert.That(configuration.PackageType, Is.EqualTo(PackageType.NuGet));
            Assert.That(configuration.PackageSource, Is.EqualTo("d:\\test"));
            Assert.That(configuration.InstallRoot, Is.EqualTo("Apps"));
        }
    }
}
