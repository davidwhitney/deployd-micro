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

        public DeploydConfiguration DeploydConfiguration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppExtractionCommand(IFileSystem fs, IEnumerable<IPackageExtractor> extractors, DeploydConfiguration deploydConfiguration)
        {
            _fs = fs;
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

            _fs.EnsureDirectoryExists(InstanceConfiguration.AppDirectory.FullPath);
            _fs.EnsureDirectoryExists(InstanceConfiguration.AppDirectory.Staging);

            var packageInfo = InstanceConfiguration.PackageLocation.PackageDetails;
            var extractor = GetExtractorFor(packageInfo);

            extractor.Unpack(InstanceConfiguration.AppDirectory.Staging, packageInfo);
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
