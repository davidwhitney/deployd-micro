using System.Collections.Generic;
using System.Linq;
using NuGet;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd_package.Features.IncludedFileLocation
{
    public class PackageFileLocator
    {
        private readonly IFileSystem _fs;

        public PackageFileLocator(IFileSystem fs)
        {
            _fs = fs;
        }

        public IEnumerable<IPackageFile> IncludedFiles(string rootDirectory)
        {
            var directories = _fs.Directory.GetDirectories(rootDirectory).ToList();
            directories.Add(rootDirectory);

            foreach (var directory in directories)
            {
                var files = _fs.Directory.GetFiles(directory);
                
                foreach (var file in files)
                {
                    yield return new PhysicalPackageFile
                        {
                            SourcePath = file,
                            TargetPath = file.Replace(rootDirectory + "\\", string.Empty),
                        };
                }
            }
        }
    }
}