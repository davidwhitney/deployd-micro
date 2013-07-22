using System;
using System.IO;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class PackageCache : IPackageCache
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly ILog _log;
        private readonly InstanceConfiguration _instanceConfiguration;
        private readonly IApplicationMap _applicationMap;

        public PackageCache(System.IO.Abstractions.IFileSystem fs, ILog log, 
            InstanceConfiguration instanceConfiguration, IApplicationMap applicationMap)
        {
            _fs = fs;
            _log = log;
            _instanceConfiguration = instanceConfiguration;
            _applicationMap = applicationMap;
        }

        private const int DownloadChunkSize = 4096;
        public IPackage CachePackage(IPackage package)
        {
            if (!_fs.Directory.Exists(_applicationMap.CachePath))
            {
                _fs.Directory.CreateDirectory(_applicationMap.CachePath);
            }

            string packageFileName = string.Format("{0}.{1}.nupkg", package.Id, package.Version);
            string packagePath = _fs.Path.Combine(_applicationMap.CachePath, packageFileName);
            if (!PackageIsCached(_applicationMap, package)
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

        private bool PackageIsCached(IApplicationMap applicationMap, IPackage package)
        {
            if (!_fs.Directory.Exists(applicationMap.CachePath))
            {
                return false;
            }

            string packageFileName = string.Format("{0}.{1}.nupkg", package.Id, package.Version);
            string packagePath = _fs.Path.Combine(applicationMap.CachePath, packageFileName);
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