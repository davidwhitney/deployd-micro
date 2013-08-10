using System.IO;
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

        [SetUp]
        public void SetUp()
        {
            _fs = new Mock<IFileSystem>();
            _cfgManager = new DeploydConfigurationManager(_fs.Object);
        }

        [Test]
        public void LoadConfig_NoConfigFileFound_LoadsDefaultConfiguration()
        {
            var fakeFileStream = new MemoryStream();
            _fs.Setup(x => x.File.Exists("config.json")).Returns(false);
            _fs.Setup(x => x.File.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>())).Returns(fakeFileStream);
            _fs.Setup(x => x.File.ReadAllText(It.IsAny<string>())).Returns(()=>Encoding.UTF8.GetString(fakeFileStream.ToArray()));

            var configuration = _cfgManager.LoadConfig();

            Assert.That(configuration.DefaultConfiguration, Is.True);
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

            Assert.That(configuration.DefaultConfiguration, Is.False);
            Assert.That(configuration.PackageType, Is.EqualTo(PackageType.NuGet));
            Assert.That(configuration.PackageSource, Is.EqualTo("d:\\test"));
            Assert.That(configuration.InstallRoot, Is.EqualTo("Apps"));
        }
    }
}
