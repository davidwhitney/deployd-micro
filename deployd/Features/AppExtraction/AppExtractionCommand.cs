using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.AppConfiguration;
using deployd.Features.AppLocating;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IApplicationFactory _appFactory;
        private readonly IInstallationRoot _installRoot;
        private readonly IEnumerable<IApplicationConfigurator> _configurators;
        private readonly IList<IPackageExtractor> _extractors;
        private readonly IInstanceConfiguration _config;

        public AppExtractionCommand(IEnumerable<IPackageExtractor> extractors, 
            IInstanceConfiguration config,
            IApplicationFactory appFactory, 
            IInstallationRoot installRoot,
            IEnumerable<IApplicationConfigurator> configurators )
        {
            _appFactory = appFactory;
            _installRoot = installRoot;
            _configurators = configurators;
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

            var configurer = GetConfiguratorFor(packageInfo);
            if (configurer != null)
            {
                configurer.Configure(_config.ApplicationMap.Staging, packageInfo, _config.Environment);
            }
        }

        private IApplicationConfigurator GetConfiguratorFor(object packageInfo)
        {
            var configurator = _configurators.FirstOrDefault(x => x.CanConfigure(packageInfo, _config));
            return configurator;
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
