using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using deployd.Features.AppLocating;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;
        private readonly InstallationPadLock _installationLock;
        private readonly IList<IPackageExtractor> _extractors;
        private readonly DeploydConfiguration _deploydConfiguration;
        private readonly IInstanceConfiguration _config;

        public AppExtractionCommand(IFileSystem fs, 
            IEnumerable<IPackageExtractor> extractors, 
            DeploydConfiguration deploydConfiguration, 
            IInstanceConfiguration config, 
            ILog log,
            InstallationPadLock installationLock)
        {
            _fs = fs;
            _log = log;
            _installationLock = installationLock;
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
            _config.ApplicationMap.For(_config.AppName, appDirectory);

            _log.Info("Active App directory: " + _config.ApplicationMap.FullPath);

            _fs.EnsureDirectoryExists(_config.ApplicationMap.FullPath);
            _fs.EnsureDirectoryExists(_config.ApplicationMap.Staging);

            _installationLock.LockAppInstallation();

            var packageInfo = _config.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            _log.Info("Unpacking into staging area...");

            extractor.Unpack(_config.ApplicationMap.Staging, packageInfo);
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
