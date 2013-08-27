using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using NuGet;
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
            var di = _fs.DirectoryInfo.FromDirectoryName(rootDirectory);
            var files = di.GetFiles("*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                yield return new PhysicalPackageFile
                {
                    SourcePath = file.FullName,
                    TargetPath = file.FullName.Replace(rootDirectory + "\\", string.Empty),
                };
            }
        }
    }
}