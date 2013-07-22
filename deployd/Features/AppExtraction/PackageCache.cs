using System;
using System.IO;
using NuGet;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class PackageCache : IPackageCache
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly ILog _log;
        private readonly InstanceConfiguration _instanceConfiguration;

        public PackageCache(System.IO.Abstractions.IFileSystem fs, ILog log, InstanceConfiguration instanceConfiguration)
        {
            _fs = fs;
            _log = log;
            _instanceConfiguration = instanceConfiguration;
        }

        private const int DownloadChunkSize = 4096;
        public IPackage CachePackage(IPackage package)
        {
            string cachePath = _fs.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

            if (!_fs.Directory.Exists(cachePath))
            {
                _fs.Directory.CreateDirectory(cachePath);
            }

            string packageFileName = string.Format("{0}.{1}.nupkg", package.Id, package.Version);
            string packagePath = _fs.Path.Combine(cachePath, packageFileName);
            if (!PackageIsCached(package)
                || _instanceConfiguration.ForceDownload)
            {
                _log.Info("Downloading package from source...");
                using (
                    var fileStream = _fs.File.Open(packagePath, FileMode.Create, FileAccess.Write, FileShare.None)
                    )
                {
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

        private bool PackageIsCached(IPackage package)
        {
            string cachePath = _fs.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");

            if (!_fs.Directory.Exists(cachePath))
            {
                return false;
            }

            string packageFileName = string.Format("{0}.{1}.nupkg", package.Id, package.Version);
            string packagePath = _fs.Path.Combine(cachePath, packageFileName);
            if (_fs.File.Exists(packagePath))
            {
                _log.Debug("Package exists in cache");
                ZipPackage cachedPackage = null;
                try
                {
                    cachedPackage = new ZipPackage(packagePath);
                }
                catch (Exception)
                {
                    _log.Debug("Package is corrupted");
                    return false;
                }
                _log.DebugFormat("Package version is {0}, cached version is {1}", package.Version, cachedPackage.Version);
                return cachedPackage.Version == package.Version;
            }

            return false;
        }
    }
}