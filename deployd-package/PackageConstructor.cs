using System.Collections.Generic;
using NuGet;

namespace deployd_package
{
    public class PackageConstructor
    {
        public static PackageBuilder BuildPackage(IEnumerable<IPackageFile> packageFiles, PackageMetadata metaData)
        {
            var package = new PackageBuilder
            {
                Id = metaData.Id,
                Version = metaData.Version,
                Description = metaData.Description
            };

            foreach (var item in metaData.Authors)
            {
                package.Authors.Add(item);
            }

            foreach (var file in packageFiles)
            {
                package.Files.Add(file);
            }

            return package;
        }
    }
}
