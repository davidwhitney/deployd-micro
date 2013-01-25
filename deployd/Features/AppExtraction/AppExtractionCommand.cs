using System;
using System.IO;
using System.IO.Abstractions;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using deployd.Infrastructure;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppExtractionCommand(IFileSystem fs, Configuration configuration)
        {
            _fs = fs;
            Configuration = configuration;

            var installRoot = configuration.InstallRoot.ToAbsolutePath();
            if (!_fs.Directory.Exists(installRoot))
            {
                _fs.Directory.CreateDirectory(installRoot);
            }
        }

        public void Execute()
        {
            if (InstanceConfiguration.AppInstallationLocation == null)
            {
                return;
            }

            var appDirectory = Path.Combine(Configuration.InstallRoot, InstanceConfiguration.AppName).ToAbsolutePath();
            var installationStaging = Path.Combine(appDirectory, ".installing").ToAbsolutePath();
            
            EnsureDirectoryExists(appDirectory);
            EnsureDirectoryExists(installationStaging);

            var packageInfo = InstanceConfiguration.AppInstallationLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            extractor.Unpack(installationStaging, packageInfo);
        }

        private static IPackageExtractor GetExtractorFor(object packageInfo)
        {
            return new NuGetPackageExtractor();
        }

        public void EnsureDirectoryExists(string fullPath)
        {
            if (!_fs.Directory.Exists(fullPath))
            {
                _fs.Directory.CreateDirectory(fullPath);
            }
        }
    }
}
