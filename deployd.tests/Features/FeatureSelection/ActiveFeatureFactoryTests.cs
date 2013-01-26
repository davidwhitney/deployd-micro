using Moq;
using deployd.Features.AppExtraction;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.ClientConfiguration;
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
        private InstanceConfiguration _instanceConfig;
        private DeploydConfiguration _clientConfig;
        private Mock<ILog> _log;

        [SetUp]
        public void SetUp()
        {
            var appKernel = new AppStart.ApplicationContext(new string[0]);
            _log = new Mock<ILog>();
            _instanceConfig = new InstanceConfiguration();
            _clientConfig = new DeploydConfiguration();
            _factory = new ActiveFeatureFactory(appKernel.Kernel, _instanceConfig, _clientConfig, _log.Object);
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

        [TestCase("")]
        [TestCase("AppName")]
        public void BuildCommands_AnyCommandBuilt_ConfigurationObjectsBound(string appName)
        {
            _instanceConfig.AppName = appName;

            var commands = _factory.BuildCommands();

            foreach (var command in commands)
            {
                Assert.That(command.DeploydConfiguration, Is.EqualTo(_clientConfig));
                Assert.That(command.InstanceConfiguration, Is.EqualTo(_instanceConfig));
            }
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
