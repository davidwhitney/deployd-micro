using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.AppLocating;
using deployd.Features.Environment;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IApplicationFactory _appFactory;
        private readonly IInstallationRoot _installRoot;
        private readonly IEnumerable<IEnvironmentApplier> _configurators;
        private readonly TextWriter _output;
        private readonly IFileSystem _fs;
        private readonly IList<IPackageExtractor> _extractors;
        private readonly IInstanceConfiguration _config;

        public AppExtractionCommand(IEnumerable<IPackageExtractor> extractors, 
            IInstanceConfiguration config,
            IApplicationFactory appFactory, 
            IInstallationRoot installRoot,
            IEnumerable<IEnvironmentApplier> configurators,
            TextWriter output,
            IFileSystem fs)
        {
            _appFactory = appFactory;
            _installRoot = installRoot;
            _configurators = configurators;
            _output = output;
            _fs = fs;
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

            _config.ApplicationMap.Configure(_config.AppName, _installRoot.Path);
            
            var currentApp = _appFactory.GetCurrent();
            currentApp.EnsureDataDirectoriesExist();

            // if desired version is already staged then no need to unpack
            if (!UnpackIsRequired()
                 && !_config.ForceUnpack)
            {
                _output.WriteLine("Skipping package unpacking as version {0} is already staged. Force unpack by specifying the -fu argument.",
                    _config.Version);
                return;
            }

            currentApp.LockForInstall();

            var packageInfo = _config.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            extractor.Unpack(_config.ApplicationMap.Staging, packageInfo);

            var environmentApplier = GetEnvironmentApplierFor(packageInfo);
            if (environmentApplier != null)
            {
                environmentApplier.Apply(_config.ApplicationMap.Staging, packageInfo, _config.Environment);
            }

            WriteUpdatedStagingManifest(_config.Version.ToString());

            _output.WriteLine("{0} {1} extracted to staging folder", _config.AppName, _config.Version);
        }

        private bool UnpackIsRequired()
        {
            if (!_fs.File.Exists(_config.ApplicationMap.StagingVersionFile)) 
                return true;
            
            string stagedVersion = _fs.File.ReadAllText(_config.ApplicationMap.StagingVersionFile);
            if (_config.Version.Equals(stagedVersion))
            {
                return false;
            }
            return true;
        }

        private void WriteUpdatedStagingManifest(string stagedVersion)
        {
            _fs.File.WriteAllText(_config.ApplicationMap.StagingVersionFile, stagedVersion);
        }


        private IEnvironmentApplier GetEnvironmentApplierFor(object packageInfo)
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
