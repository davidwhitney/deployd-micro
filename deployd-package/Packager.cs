using System;
using System.IO;
using NuGet;
using deployd_package.Features.IncludedFileLocation;
using deployd_package.Features.MetadataDiscovery;
using deployd_package.Features.PackageBuilding;

namespace deployd_package
{
    public class Packager
    {
        private readonly PackageFileLocator _packageFileLocator;
        private readonly PackageMetadataLocator _metaDataLocator;
        private readonly PackageConstructor _constructor;

        public Packager(PackageFileLocator packageFileLocator, PackageMetadataLocator metaDataLocator, PackageConstructor constructor)
        {
            _packageFileLocator = packageFileLocator;
            _metaDataLocator = metaDataLocator;
            _constructor = constructor;
        }

        public void Package(string source, string destination, Version forcedVersion=null)
        {
            var includedFiles = _packageFileLocator.IncludedFiles(source);
            var metaData = _metaDataLocator.DiscoverPackageMetadata(source);
            var package = _constructor.BuildPackage(includedFiles, metaData);
            if (forcedVersion != null)
                metaData.Version = new SemanticVersion(forcedVersion);

            var outputLocation = Path.Combine(destination, metaData.PackageFilename);

            using (var fs = new FileStream(outputLocation, FileMode.Create))
            {
                package.Save(fs);
                fs.Close();
            }
        }
    }
}