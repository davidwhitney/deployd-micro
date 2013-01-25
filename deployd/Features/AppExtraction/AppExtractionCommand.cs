using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using deployd.Infrastructure;

namespace deployd.Features.AppExtraction
{
    public class AppExtractionCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly IList<IPackageExtractor> _extractors;

        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppExtractionCommand(IFileSystem fs, IEnumerable<IPackageExtractor> extractors, Configuration configuration)
        {
            _fs = fs;
            _extractors = extractors.ToList();
            Configuration = configuration;

            var installRoot = configuration.InstallRoot.ToAbsolutePath();
            _fs.EnsureDirectoryExists(installRoot);
        }

        public void Execute()
        {
            if (InstanceConfiguration.AppInstallationLocation == null)
            {
                return;
            }

            var appDirectory = Path.Combine(Configuration.InstallRoot, InstanceConfiguration.AppName).ToAbsolutePath();
            var installationStaging = Path.Combine(appDirectory, ".staging").ToAbsolutePath();
            
            _fs.EnsureDirectoryExists(appDirectory);
            _fs.EnsureDirectoryExists(installationStaging);

            var packageInfo = InstanceConfiguration.AppInstallationLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            extractor.Unpack(installationStaging, packageInfo);
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
