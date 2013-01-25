using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Ninject;
using deployd.Features;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using deployd.Features.Help;

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

        [Test]
        public void BuildCommands_AppNameSpecifiedAndHelpCommandOmitted_DeploymentCommandsBuilt()
        {
            _instanceConfig.AppName = "MyApp";

            var commands = _factory.BuildCommands();

            Assert.That(commands.FirstOrDefault(x => x.GetType() == typeof (HelpCommand)), Is.Null);
        }
    }
}
