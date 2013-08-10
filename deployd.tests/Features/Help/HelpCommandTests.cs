using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
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
            var outputStream = new MemoryStream();
            var log = new Mock<ILog>();
            log.Setup(x => x.Info(It.IsAny<string>()));
            var config = new ArgumentParser().Parse(new List<string>());
            var cmd = new HelpCommand(config, log.Object, outputStream);

            cmd.Execute();
            string output = Encoding.UTF8.GetString(outputStream.ToArray());
            Debug.WriteLine(output);
            Assert.That(output, Is.Not.Empty);
        }

        [Test]
        public void Execute_PrintsSomethingUseful()
        {
            var outputStream = new MemoryStream();
            var log = new Mock<ILog>();
            log.Setup(x => x.Info(It.IsAny<string>()));

            var config = new ArgumentParser().Parse(new List<string>());
            var cmd = new HelpCommand(config, log.Object, outputStream);

            cmd.Execute();

            string output = Encoding.UTF8.GetString(outputStream.ToArray());
            Debug.WriteLine(output);
            Assert.That(output, Is.StringContaining("-app"));
            Assert.That(output, Is.StringContaining("-v"));
            Assert.That(output, Is.StringContaining("-help"));
            Assert.That(output, Is.StringContaining("-install"));
            Assert.That(output, Is.StringContaining("-i"));
        }
    }
}
