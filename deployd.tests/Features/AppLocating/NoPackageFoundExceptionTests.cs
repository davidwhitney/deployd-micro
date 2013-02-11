using NUnit.Framework;
using deployd.Features.AppLocating;

namespace deployd.tests.Features.AppLocating
{
    [TestFixture]
    class NoPackageFoundExceptionTests
    {
        [Test]
        public void WhenExceptionIsCreated_AppNameIsUsedToGenerateMessage()
        {
            var ex = new NoPackageFoundException("AppNameHere");

            Assert.That(ex.Message, Is.EqualTo("No package matching 'AppNameHere' found."));
        }
    }
}
