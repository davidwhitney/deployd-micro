using System.IO.Abstractions.TestingHelpers;
using deployd.Extensibility.Configuration;
using deployd.Features;
using deployd.Features.FeatureSelection;
using NUnit.Framework;

namespace deployd.tests.Features.ClientConfiguration
{
    [TestFixture]
    public class InstallationPathTests
    {
        [Test]
        public void WhenInstallRootIsConfigured_InstallationPathIsInstallRootPlusApplicationName()
        {
            var configuration=new DeploydConfiguration()
            {
                InstallRoot=@"c:\"
            };
            var instance=new InstanceConfiguration(){AppName="SomeApplication"};
            var fileSystem=new MockFileSystem();
            var installationRoot = new InstallationRoot(configuration, instance, fileSystem);

            Assert.That(installationRoot.Path, Is.EqualTo(@"c:\SomeApplication"));
        }

        [Test]
        public void WhenInstallPathIsParameterisedAbsolutePath_InstallationPathIsExactlyInstallPathParameter()
        {
            var configuration = new DeploydConfiguration();
            var instance = new InstanceConfiguration() { AppName = "SomeApplication", InstallPath=@"c:\installfolder" };
            var fileSystem = new MockFileSystem();
            var installationRoot = new InstallationRoot(configuration, instance, fileSystem);

            Assert.That(installationRoot.Path, Is.EqualTo(@"c:\installfolder"));
        }

        [Test]
        public void WhenInstallPathIsParameterisedRelativePath_InstallationPathIsRelativeToConfiguredPath()
        {
            var configuration = new DeploydConfiguration(){InstallRoot=@"c:\"};
            var instance = new InstanceConfiguration() { AppName = "SomeApplication", InstallPath = @"relativepath" };
            var fileSystem = new MockFileSystem();
            var installationRoot = new InstallationRoot(configuration, instance, fileSystem);

            Assert.That(installationRoot.Path, Is.EqualTo(@"c:\relativepath"));
        }

        [Test]
        public void WhenInstallPathIsParameterisedAbsolutePath_ArgumentOverridesConfiguration()
        {
            var configuration = new DeploydConfiguration(){InstallRoot=@"c:\"};
            var instance = new InstanceConfiguration() { AppName = "SomeApplication", InstallPath = @"d:\installfolder" };
            var fileSystem = new MockFileSystem();
            var installationRoot = new InstallationRoot(configuration, instance, fileSystem);

            Assert.That(installationRoot.Path, Is.EqualTo(@"d:\installfolder"));
        }
    }
}