using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;
        private readonly IList<IPackageExtractor> _extractors;

        public DeploydConfiguration DeploydConfiguration { get; set; }
        public IInstanceConfiguration Config { get; set; }

        public AppExtractionCommand(IFileSystem fs, IEnumerable<IPackageExtractor> extractors, DeploydConfiguration deploydConfiguration, ILog log)
        {
            _fs = fs;
            _log = log;
            _extractors = extractors.ToList();
            DeploydConfiguration = deploydConfiguration;

            var installRoot = deploydConfiguration.InstallRoot.ToAbsolutePath();
            _fs.EnsureDirectoryExists(installRoot);
        }

        public void Execute()
        {
            if (Config.PackageLocation == null)
            {
                return;
            }
            
            var appDirectory = Path.Combine(DeploydConfiguration.InstallRoot, Config.AppName).ToAbsolutePath();
            Config.ApplicationMap = new ApplicationMap(Config.AppName, appDirectory);
            
            _log.Info("Active App directory: " + Config.ApplicationMap.FullPath);

            _fs.EnsureDirectoryExists(Config.ApplicationMap.FullPath);
            _fs.EnsureDirectoryExists(Config.ApplicationMap.Staging);

            LockInstall();

            var packageInfo = Config.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            _log.Info("Unpacking into staging area...");

            extractor.Unpack(Config.ApplicationMap.Staging, packageInfo);
        }

        private void LockInstall()
        {
            if (!_fs.File.Exists(Config.ApplicationMap.Lockfile))
            {
                _fs.File.WriteAllText(Config.ApplicationMap.Lockfile, string.Empty);
            }

            Config.ApplicationMap.Lock = File.Open(Config.ApplicationMap.Lockfile,
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
