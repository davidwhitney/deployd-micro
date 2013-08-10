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
        private readonly DeploydConfiguration _configuration;
        private readonly IFileSystem _fs;

        private readonly ILog _log;

        public FileSystemAppInstallationLocator(DeploydConfiguration configuration, IFileSystem fs, ILog log)
        {
            _configuration = configuration;
            _fs = fs;
            _log = log;
        }

        public FileSystemAppInstallationLocator(string packageSource, IFileSystem fs, ILog log):
            this(new DeploydConfiguration(){PackageSource = packageSource}, fs, log)
        {
        }

        public bool SupportsPathType()
        {
            return _fs.Directory.Exists(_configuration.PackageSource);
        }

        public PackageLocation<PackagePointer> CanFindPackage(string appName)
        {
            try
            {
                var searchPath = _configuration.PackageSource;
                var matching =
                    _fs.Directory.GetFiles(searchPath, appName + ".*.zip", SearchOption.AllDirectories)
                       .OrderBy(x => x)
                       .ToList();

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