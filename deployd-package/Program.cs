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
            var context = new ApplicationContext();

            if (args.Length < 1)
            {
                Console.WriteLine("No path supplied.");
            }

            var packageSourceRoot = args[0];
            var directoryInspector = context.Kernel.Get<PackageFileLocator>();
            var metaDataLocator = context.Kernel.Get<PackageMetadataLocator>();
            var packageConstructor = context.Kernel.Get<PackageConstructor>();

            var includedFiles = directoryInspector.IncludedFiles(packageSourceRoot);
            var metaData = metaDataLocator.DiscoverPackageMetadata(packageSourceRoot);
            var package = packageConstructor.BuildPackage(includedFiles, metaData);

            using (var fs = new FileStream(metaData.PackageFilename, FileMode.Create))
            {
                package.Save(fs);
                fs.Close();
            }
        }     
    }
}
