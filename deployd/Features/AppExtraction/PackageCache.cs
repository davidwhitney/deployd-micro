using System;
using System.IO;
using NuGet;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class PackageCache : IPackageCache
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly ILog _log;

        public PackageCache(System.IO.Abstractions.IFileSystem fs, ILog log)
        {
            _fs = fs;
            _log = log;
        }

        public IPackage CachePackage(IPackage package)
        {
            string cachePath = _fs.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

            if (!_fs.Directory.Exists(cachePath))
            {
                _fs.Directory.CreateDirectory(cachePath);
            }

            string packageFileName = string.Format("{0}.{1}.nupkg", package.Id, package.Version);
            string packagePath = _fs.Path.Combine(cachePath, packageFileName);
            if (!_fs.File.Exists(packagePath))
            {
                _log.Info("Downloading package from source...");
                using (
                    var fileStream = _fs.File.Open(packagePath, FileMode.Create, FileAccess.Write, FileShare.None)
                    )
                {
                    // this should download in chunks
                    var packageData = package.GetStream().ReadAllBytes();
                    fileStream.Write(packageData, 0, packageData.Length);
                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            else
            {
                _log.Info("Using cached copy of package...");
            }
            _log.DebugFormat("Local copy is {0}", packagePath);
            return new ZipPackage(packagePath);
        }
    }
}