using System;
using System.IO;
using System.IO.Abstractions;
using deployd_package.Features.IncludedFileLocation;
using deployd_package.Features.MetadataDiscovery;
using deployd_package.Features.PackageBuilding;

namespace deployd_package
{
    class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No path supplied.");
            }

            var packageSourceRoot = args[0];

            var directoryInspector = new PackageFileLocator(new FileSystem(), packageSourceRoot);
            var metaDataLocator = new PackageMetadataLocator();

            var includedFiles = directoryInspector.IncludedFiles();
            var metaData = metaDataLocator.DiscoverPackageMetadata(packageSourceRoot);
            var package = PackageConstructor.BuildPackage(includedFiles, metaData);

            using (var fs = new FileStream(metaData.PackageFilename, FileMode.Create))
            {
                package.Save(fs);
                fs.Close();
            }
        }
    }
}
