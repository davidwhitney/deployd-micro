using Moq;
using deployd.Extensibility.Configuration;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.FeatureSelection;
using deployd.Features.Help;
using NUnit.Framework;
using System.Linq;
using log4net;

namespace deployd.tests.Features.FeatureSelection
{
    [TestFixture]
    public class ActiveFeatureFactoryTests
    {
        private ActiveFeatureFactory _factory;
        private IInstanceConfiguration _instanceConfig;
        private Mock<ILog> _log;

        [SetUp]
        public void SetUp()
        {
            var appKernel = new AppStart.ApplicationContext(new string[0]);
            _log = new Mock<ILog>();
            _instanceConfig = new InstanceConfiguration();
            _factory = new ActiveFeatureFactory(appKernel.Kernel, _instanceConfig, _log.Object);
        }

        [Test]
        public void BuildCommands_HelpIsRequested_HelpCommandBuilt()
        {
            _instanceConfig.Help = true;

            var commands = _factory.BuildCommands();

            Assert.That(commands.FirstOrDefault(x => x.GetType() == typeof (HelpCommand)), Is.Not.Null);
        }

        [Test]
        public void BuildCommands_NoAppNameSpecified_HelpCommandBuilt()
        {
            _instanceConfig.AppName = null;

            var commands = _factory.BuildCommands();

            Assert.That(commands.FirstOrDefault(x => x.GetType() == typeof (HelpCommand)), Is.Not.Null);
        }

        [Test]
        public void BuildCommands_AppNameSpecifiedAndInstallCommandOmitted_HelpProvided()
        {
            _instanceConfig.AppName = "MyApp";

            var commands = _factory.BuildCommands();

            Assert.That(commands.First().GetType(), Is.EqualTo(typeof(HelpCommand)));
        }

        [Test]
        public void BuildCommands_AppNameSpecifiedAndInstallSupplied_DeploymentCommandsBuilt()
        {
            _instanceConfig.AppName = "MyApp";
            _instanceConfig.Install = true;

            var commands = _factory.BuildCommands().ToList();

            Assert.That(commands.First().GetType(), Is.EqualTo(typeof(AppLocatingCommand)));
            Assert.That(commands.Skip(1).First().GetType(), Is.EqualTo(typeof(AppExtractionCommand)));
            Assert.That(commands.Skip(2).First().GetType(), Is.EqualTo(typeof(AppInstallationCommand)));
        }
    }
}
