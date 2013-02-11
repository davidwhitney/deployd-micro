using System.IO.Abstractions;
using deployd.Extensibility.Configuration;
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

        public ApplicationFactory(IFileSystem fs, InstallationPadLock padLock, IInstanceConfiguration config, ILog log)
        {
            _fs = fs;
            _padLock = padLock;
            _config = config;
            _log = log;
        }

        public Application GetCurrent()
        {
            return new Application(_config.ApplicationMap, _fs, _log, _config, _padLock);
        }
    }
}