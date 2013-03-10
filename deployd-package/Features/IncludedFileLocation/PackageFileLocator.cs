using System.Collections.Generic;
using System.Linq;
using NuGet;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd_package.Features.IncludedFileLocation
{
    public class PackageFileLocator
    {
        private readonly IFileSystem _fs;
        private readonly string _rootDirectory;

        public PackageFileLocator(IFileSystem fs, string rootDirectory)
        {
            _fs = fs;
            _rootDirectory = rootDirectory;
        }

        public IEnumerable<IPackageFile> IncludedFiles()
        {
            var directories = _fs.Directory.GetDirectories(_rootDirectory).ToList();
            directories.Add(_rootDirectory);

            foreach (var directory in directories)
            {
                var files = _fs.Directory.GetFiles(directory);
                
                foreach (var file in files)
                {
                    yield return new PhysicalPackageFile
                        {
                            SourcePath = file,
                            TargetPath = file.Replace(_rootDirectory + "\\", string.Empty),
                        };
                }
            }
        }
    }
}