using System.IO.Abstractions;

namespace deployd_package.tests.Features.IncludedFileLocation
{
    public class DiFactoryStub : IDirectoryInfoFactory
    {
        private readonly DirectoryInfoBase _returnThis;

        public DiFactoryStub(DirectoryInfoBase returnThis)
        {
            _returnThis = returnThis;
        }

        public DirectoryInfoBase FromDirectoryName(string directoryName)
        {
            return _returnThis;
        }
    }
}