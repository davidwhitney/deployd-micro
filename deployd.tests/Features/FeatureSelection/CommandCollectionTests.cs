using System;
using Moq;
using NUnit.Framework;
using deployd.Features;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.tests.Features.FeatureSelection
{
    [TestFixture]
    public class CommandCollectionTests
    {
        private CommandCollection _collection;
        private Mock<ILog> _log;
        private Mock<IInstallationPadLock> _padlock;
        private Mock<IFeatureCommand> _command;

        [SetUp]
        public void SetUp()
        {
            _log = new Mock<ILog>();
            _padlock = new Mock<IInstallationPadLock>();
            _collection = new CommandCollection(_log.Object, _padlock.Object);

            _command = new Mock<IFeatureCommand>();
        }

        [Test]
        public void PackageNotFoundThrown_ReturnCodeIsMinusTwo()
        {
            _command.Setup(x => x.Execute()).Throws(new NoPackageFoundException("App"));
            _collection.Add(_command.Object);

            var returnCode = _collection.RunAll();

            Assert.That(returnCode, Is.EqualTo(-2));
        }

        [Test]
        public void UnknownExceptionThrown_ReturnCodeIsMinusOne()
        {
            _command.Setup(x => x.Execute()).Throws(new Exception());
            _collection.Add(_command.Object);

            var returnCode = _collection.RunAll();

            Assert.That(returnCode, Is.EqualTo(-1));
        }

        [Test]
        public void AllCommandsRun_ReturnCodeIsZero()
        {
            _command.Setup(x => x.Execute());
            _collection.Add(_command.Object);

            var returnCode = _collection.RunAll();

            Assert.That(returnCode, Is.EqualTo(0));
        }
    }
}
