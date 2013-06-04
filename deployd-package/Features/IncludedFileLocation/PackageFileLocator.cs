using System.Collections.Generic;
using System.Linq;
using NuGet;
using log4net;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd_package.Features.IncludedFileLocation
{
    public class PackageFileLocator
    {
        private readonly IFileSystem _fs;
        private readonly ILog _log;

        public PackageFileLocator(IFileSystem fs, ILog log)
        {
            _fs = fs;
            _log = log;
        }

        public IEnumerable<IPackageFile> IncludedFiles(string rootDirectory)
        {
            var includedFiles = new HashSet<PhysicalPackageFile>();
            IncludeFilesRecursively(rootDirectory, _fs.Path.GetFullPath(rootDirectory), ref includedFiles);

            return includedFiles;
        }

        private void IncludeFilesRecursively(string path, string pathToIgnore, ref HashSet<PhysicalPackageFile> includedFiles)
        {
            var directories = _fs.Directory.GetDirectories(path).ToList();

            foreach (var directory in directories)
            {
                IncludeFilesRecursively(directory, pathToIgnore, ref includedFiles);
            }

            var files = _fs.Directory.GetFiles(path);

            foreach (var file in files)
            {
                _log.DebugFormat("Including {0}", file);
                var physicalPackageFile = new PhysicalPackageFile
                    {
                        SourcePath = file,
                        TargetPath = _fs.Path.GetFullPath(file).Substring(pathToIgnore.Length),
                    };
                if (!includedFiles.Contains(physicalPackageFile))
                    includedFiles.Add(physicalPackageFile);
            }

        }
    }
}