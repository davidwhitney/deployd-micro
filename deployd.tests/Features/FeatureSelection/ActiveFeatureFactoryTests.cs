using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using deployd.Features.Help;
using Moq;
using Ninject;
using NUnit.Framework;
using System.Linq;

namespace deployd.tests.Features.FeatureSelection
{
    [TestFixture]
    public class ActiveFeatureFactoryTests
    {
        private ActiveFeatureFactory _factory;
        private Mock<IKernel> _kernel;
        private InstanceConfiguration _instanceConfig;
        private Configuration _clientConfig;

        [SetUp]
        public void SetUp()
        {
            _kernel = new Mock<IKernel>();
            _kernel.Setup(x => x.GetService(typeof(HelpCommand))).Returns(new HelpCommand());

            _instanceConfig = new InstanceConfiguration();
            _clientConfig = new Configuration();
            _factory = new ActiveFeatureFactory(_kernel.Object, _instanceConfig, _clientConfig);
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
                Assert.That(command.Configuration, Is.EqualTo(_clientConfig));
                Assert.That(command.InstanceConfiguration, Is.EqualTo(_instanceConfig));
            }
        }

        [Test]
        public void BuildCommands_AppNameSpecifiedAndHelpCommandOmitted_HelpNotProvided()
        {
            _instanceConfig.AppName = "MyApp";

            var commands = _factory.BuildCommands();

            Assert.That(commands.FirstOrDefault(x => x.GetType() == typeof (HelpCommand)), Is.Null);
        }

        [Test]
        public void BuildCommands_AppNameSpecifiedAndHelpCommandOmitted_DeploymentCommandsBuilt()
        {
            _instanceConfig.AppName = "MyApp";

            var commands = _factory.BuildCommands();

            Assert.Inconclusive("Need to build chains of commands.");
        }
    }
}
