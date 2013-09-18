using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using deployd.Extensibility.Configuration;
using deployd.Features.FeatureSelection;

namespace deployd.tests.Features.ConfigureCommand
{
    [TestFixture]
    public class ConfigureCommandTests
    {
        [TestCase("PackageType=nuget")]
        [TestCase("PackageSource=http://some/nuget/feed/url")]
        [TestCase("InstallRoot=c:\\some\\drive\\path")]
        public void CanSetPackageTypeOptionFromAString(string configCommand)
        {
            var configFileStream = new MemoryStream();
            var fileSystem = new Mock<IFileSystem>();
            fileSystem.SetupGet(x => x.Path).Returns(new MockPath(new MockFileSystem()));
            var appFolderLocator = new Mock<IApplicationFolderLocator>();
            appFolderLocator.SetupGet(x=>x.ApplicationFolder).Returns("c:\\");
            fileSystem.Setup(x => x.File.Open(It.IsAny<string>(), FileMode.Create, FileAccess.Write)).Returns(configFileStream);

            IInstanceConfiguration instanceConfiguration=new InstanceConfiguration()
                {
                    SetConfigurationValue = configCommand
                };

            var deploydConfiguration=new DeploydConfiguration();
            var configurationManager = new DeploydConfigurationManager(fileSystem.Object, appFolderLocator.Object);
            TextWriter output = new StringWriter(new StringBuilder());
            var command = new deployd.Features.AppConfiguration.ConfigureCommand(instanceConfiguration,
                                                                                 deploydConfiguration,
                                                                                 configurationManager, output);
            command.Execute();

            Assert.That(deploydConfiguration.PackageType, Is.EqualTo(PackageType.NuGet));
        }
    }
}
