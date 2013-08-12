using System.IO;
using System.IO.Abstractions;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using log4net;

namespace deployd.Features
{
    public interface IApplicationFactory
    {
        Application GetCurrent();
    }

    public class ApplicationFactory : IApplicationFactory
    {
        private readonly IFileSystem _fs;
        private readonly InstallationPadLock _padLock;
        private readonly IInstanceConfiguration _config;
        private readonly ILog _log;
        private readonly TextWriter _output;
        private readonly Stream _outputStream;

        public ApplicationFactory(IFileSystem fs, InstallationPadLock padLock, IInstanceConfiguration config, ILog log, TextWriter output)
        {
            _fs = fs;
            _padLock = padLock;
            _config = config;
            _log = log;
            _output = output;
        }

        public Application GetCurrent()
        {
            return new Application(_config.ApplicationMap, _fs, _log, _config, _padLock, _output);
        }
    }
}