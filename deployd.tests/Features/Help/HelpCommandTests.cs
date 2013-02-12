using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using deployd.Features.FeatureSelection;
using deployd.Features.Help;
using log4net;

namespace deployd.tests.Features.Help
{
    [TestFixture]
    public class HelpCommandTests
    {
        [Test]
        public void Execute_PrintsSomethingToTheLog()
        {
            var log = new Mock<ILog>();
            log.Setup(x => x.Info(It.IsAny<string>()));
            var config = new ArgumentParser().Parse(new List<string>());
            var cmd = new HelpCommand(config, log.Object);

            cmd.Execute();

            log.Verify(x=>x.Info(It.IsAny<string>()));
        }

        [Test]
        public void Execute_PrintsSomethingUseful()
        {
            var log = new Mock<ILog>();
            log.Setup(x => x.Info(It.IsAny<string>()));

            var config = new ArgumentParser().Parse(new List<string>());
            var cmd = new HelpCommand(config, log.Object);

            cmd.Execute();

            log.Verify(x=>x.Info(It.Is<string>(y=>y.Contains("-app"))));
            log.Verify(x=>x.Info(It.Is<string>(y=>y.Contains("-v"))));
            log.Verify(x=>x.Info(It.Is<string>(y=>y.Contains("-help"))));
            log.Verify(x=>x.Info(It.Is<string>(y=>y.Contains("-install"))));
            log.Verify(x=>x.Info(It.Is<string>(y=>y.Contains("-i"))));
        }
    }
}
