using Moq;
using NUnit.Framework;
using deployd.Features;
using deployd.Features.PurgeOldBackups;

namespace deployd.tests.Features.PurgeOldBackups
{
    [TestFixture]
    public class PurgeOldBackupsCommandTests
    {
        [Test]
        public void Execute_InvokesApplicationMethod()
        {
            var app = new Mock<IApplication>();
            var cmd = new PurgeOldBackupsCommand(app.Object);

            cmd.Execute();

            app.Verify(x=>x.PruneBackups(), Times.Once());
        }
    }
}
