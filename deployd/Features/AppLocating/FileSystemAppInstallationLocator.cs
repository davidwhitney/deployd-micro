using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features.AppLocating
{
    public class FileSystemAppInstallationLocator : IAppInstallationLocator<PackagePointer>
    {
        private readonly IFileSystem _fs;
        private readonly DeploydConfiguration _configuration;
        private readonly ILog _log;

        public FileSystemAppInstallationLocator(IFileSystem fs, DeploydConfiguration configuration, ILog log)
        {
            _fs = fs;
            _configuration = configuration;
            _log = log;
        }

        public bool SupportsPathType()
        {
            return _fs.Directory.Exists(_configuration.InstallRoot);
        }

        public PackageLocation<PackagePointer> CanFindPackage(string appName)
        {
            try
            {
                var searchPath = _configuration.PackageSource;
                var matching = _fs.Directory.GetFiles(searchPath, appName + ".*.zip", SearchOption.AllDirectories).ToList();
                matching.Reverse();

                var latestPackage = matching.FirstOrDefault();

                if (latestPackage != null)
                {
                    return new PackageLocation<PackagePointer>
                        {
                            PackageDetails = new PackagePointer(latestPackage.ToAbsolutePath()),
                            PackageVersion = string.Empty
                        };
                }
            }
            catch (Exception ex)
            {
                _log.Error("Could not get package from file system", ex);
            }

            return null;
        }

        public PackageLocation<object> CanFindPackageAsObject(string appName)
        {
            var inner = CanFindPackage(appName);
            return inner == null ? null : new PackageLocation<object> { PackageDetails = inner.PackageDetails };
        }
    }
}