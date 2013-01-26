using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
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
        public InstanceConfiguration InstanceConfiguration { get; set; }

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
            if (InstanceConfiguration.PackageLocation == null)
            {
                return;
            }
            
            var appDirectory = Path.Combine(DeploydConfiguration.InstallRoot, InstanceConfiguration.AppName).ToAbsolutePath();
            InstanceConfiguration.AppDirectory = new AppDirectory(appDirectory);
            
            _log.Info("Active App directory: " + InstanceConfiguration.AppDirectory.FullPath);

            _fs.EnsureDirectoryExists(InstanceConfiguration.AppDirectory.FullPath);
            _fs.EnsureDirectoryExists(InstanceConfiguration.AppDirectory.Staging);

            LockInstall();

            var packageInfo = InstanceConfiguration.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            _log.Info("Unpacking into staging area...");

            extractor.Unpack(InstanceConfiguration.AppDirectory.Staging, packageInfo);
        }

        private void LockInstall()
        {
            if (!_fs.File.Exists(InstanceConfiguration.AppDirectory.Lockfile))
            {
                _fs.File.WriteAllText(InstanceConfiguration.AppDirectory.Lockfile, string.Empty);
            }

            InstanceConfiguration.AppDirectory.Lock = File.Open(InstanceConfiguration.AppDirectory.Lockfile,
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
