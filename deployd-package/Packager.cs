using System.IO;
using deployd_package.Features.IncludedFileLocation;
using deployd_package.Features.MetadataDiscovery;
using deployd_package.Features.PackageBuilding;
using log4net;

namespace deployd_package
{
    public class Packager
    {
        private readonly PackageFileLocator _packageFileLocator;
        private readonly PackageMetadataLocator _metaDataLocator;
        private readonly PackageConstructor _constructor;
        private readonly ILog _log;

        public Packager(PackageFileLocator packageFileLocator, PackageMetadataLocator metaDataLocator, PackageConstructor constructor, ILog log)
        {
            _packageFileLocator = packageFileLocator;
            _metaDataLocator = metaDataLocator;
            _constructor = constructor;
            _log = log;
        }

        public void Package(string source, string destination, bool verbose)
        {
            var includedFiles = _packageFileLocator.IncludedFiles(source);

            if (verbose)
            {
                foreach (var file in includedFiles)
                {
                    _log.Info(string.Format("Discovered file '{0}'", file.Path));
                }
            }

            var metaData = _metaDataLocator.DiscoverPackageMetadata(source);

            _log.Info(string.Format("PackageId: '{0}'", metaData.Id));
            _log.Info(string.Format("Description: '{0}'", metaData.Description));
            _log.Info(string.Format("Version: '{0}'", metaData.Version));
            _log.Info(string.Format("Filename: '{0}'", metaData.PackageFilename));

            var package = _constructor.BuildPackage(includedFiles, metaData);

            var outputLocation = Path.Combine(destination, metaData.PackageFilename);
            var fullOutputPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), outputLocation));
            _log.Info(string.Format("Output location: '{0}'", fullOutputPath));

            using (var fs = new FileStream(outputLocation, FileMode.Create))
            {
                package.Save(fs);
                fs.Close();
            }
        }
    }
}