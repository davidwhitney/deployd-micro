using System;
using System.IO;
using System.IO.Abstractions;

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

            var includedFiles = directoryInspector.IncludedFiles();
            var metaData = PackageMetadataLocator.DiscoverPackageMetadata(packageSourceRoot);
            var package = PackageConstructor.BuildPackage(includedFiles, metaData);

            using (var fs = new FileStream(metaData.PackageFilename, FileMode.Create))
            {
                package.Save(fs);
                fs.Close();
            }
        }
    }
}
