using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;
        private readonly IList<IPackageExtractor> _extractors;
        private readonly DeploydConfiguration _deploydConfiguration;
        private readonly IInstanceConfiguration _config;

        public AppExtractionCommand(IFileSystem fs, IEnumerable<IPackageExtractor> extractors, DeploydConfiguration deploydConfiguration, IInstanceConfiguration config, ILog log)
        {
            _fs = fs;
            _log = log;
            _extractors = extractors.ToList();
            _deploydConfiguration = deploydConfiguration;
            _config = config;

            var installRoot = deploydConfiguration.InstallRoot.ToAbsolutePath();
            _fs.EnsureDirectoryExists(installRoot);
        }

        public void Execute()
        {
            if (_config.PackageLocation == null)
            {
                throw new NoPackageFoundException(_config.AppName);
            }

            var appDirectory = Path.Combine(_deploydConfiguration.InstallRoot, _config.AppName).ToAbsolutePath();
            _config.ApplicationMap = new ApplicationMap(_config.AppName, appDirectory);

            _log.Info("Active App directory: " + _config.ApplicationMap.FullPath);

            _fs.EnsureDirectoryExists(_config.ApplicationMap.FullPath);
            _fs.EnsureDirectoryExists(_config.ApplicationMap.Staging);

            LockInstall();

            var packageInfo = _config.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            _log.Info("Unpacking into staging area...");

            extractor.Unpack(_config.ApplicationMap.Staging, packageInfo);
        }

        private void LockInstall()
        {
            if (!_fs.File.Exists(_config.ApplicationMap.Lockfile))
            {
                _fs.File.WriteAllText(_config.ApplicationMap.Lockfile, string.Empty);
            }

            _config.ApplicationMap.Lock = File.Open(_config.ApplicationMap.Lockfile,
                                                                FileMode.Open, FileAccess.Read, FileShare.None);
        }

        private IPackageExtractor GetExtractorFor(object packageInfo)
        {
            var extractor = _extractors.FirstOrDefault(x => x.CanUnpack(packageInfo));

            if (extractor == null)
            {
                throw new InvalidOperationException("No supported extractor");
            }

            return extractor;
        }
    }
}
