using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IApplicationFactory _appFactory;
        private readonly IInstallationRoot _installRoot;
        private readonly IList<IPackageExtractor> _extractors;
        private readonly IInstanceConfiguration _config;

        public AppExtractionCommand(IEnumerable<IPackageExtractor> extractors, 
            IInstanceConfiguration config,
            IApplicationFactory appFactory, 
            IInstallationRoot installRoot)
        {
            _appFactory = appFactory;
            _installRoot = installRoot;
            _extractors = extractors.ToList();
            _config = config;

            _installRoot.EnsureInstallationDirectoryExists();
        }

        public void Execute()
        {
            if (_config.PackageLocation == null)
            {
                throw new NoPackageFoundException(_config.AppName);
            }

            var appDirectory = Path.Combine(_installRoot.Path, _config.AppName).ToAbsolutePath();
            _config.ApplicationMap.Configure(_config.AppName, appDirectory);
            
            var currentApp = _appFactory.GetCurrent();
            currentApp.EnsureDataDirectoriesExist();
            currentApp.LockForInstall();

            var packageInfo = _config.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

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
