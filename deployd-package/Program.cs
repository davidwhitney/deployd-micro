using System;
using System.IO;
using Ninject;
using deployd_package.AppStart;
using deployd_package.Features.IncludedFileLocation;
using deployd_package.Features.MetadataDiscovery;
using deployd_package.Features.PackageBuilding;

namespace deployd_package
{
    class Program
    {
        private static void Main(string[] args)
        {
            var kernel = new ApplicationContext().Start();

            if (args.Length < 1)
            {
                Console.WriteLine("No path supplied.");
            }

            var packageSourceRoot = args[0];
            var directoryInspector = kernel.Get<PackageFileLocator>();
            var metaDataLocator = kernel.Get<PackageMetadataLocator>();

            var includedFiles = directoryInspector.IncludedFiles(packageSourceRoot);
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
