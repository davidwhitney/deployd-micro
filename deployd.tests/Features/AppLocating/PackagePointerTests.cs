using NUnit.Framework;
using deployd.Features.AppLocating;

namespace deployd.tests.Features.AppLocating
{
    [TestFixture]
    public class PackagePointerTests
    {
        [TestCase("zip")]
        [TestCase("ZiP")]
        [TestCase("ZIP")]
        public void Pointer_ExtensionIsZip_IsZipFileIsTrue(string extension)
        {
            var pointer = new PackagePointer("file." + extension);

            Assert.That(pointer.IsZipFile, Is.True);
        }

        [Test]
        public void Pointer_ExtensionIsNotZip_IsZipFileIsFalse()
        {
            var pointer = new PackagePointer("file.tar");

            Assert.That(pointer.IsZipFile, Is.False);
        }
    }
}
