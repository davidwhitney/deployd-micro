using System.IO.Abstractions;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features
{
    public class ApplicationFactory
    {
        private readonly IFileSystem _fs;
        private readonly IInstanceConfiguration _config;
        private readonly ILog _log;

        public ApplicationFactory(IFileSystem fs, IInstanceConfiguration config, ILog log)
        {
            _fs = fs;
            _config = config;
            _log = log;
        }

        public Application GetCurrent()
        {
            return new Application(_config.ApplicationMap, _fs, _log, _config);
        }
    }
}