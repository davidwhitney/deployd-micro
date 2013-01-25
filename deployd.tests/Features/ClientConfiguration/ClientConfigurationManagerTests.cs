using Moq;
using NUnit.Framework;
using deployd.Features.ClientConfiguration;
using System.IO.Abstractions;

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
            _fs.Setup(x => x.File.Exists("config.json")).Returns(false);

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
