using System;
using System.IO;
using System.Linq;
using NuGet;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using log4net;
using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace deployd.Features.AppLocating
{
    public class FileSystemAppInstallationLocator : IAppInstallationLocator<PackagePointer>
    {
        private readonly IPackageSourceConfiguration _packageSourceConfiguration;
        private readonly IFileSystem _fs;

        private readonly ILog _log;

        public FileSystemAppInstallationLocator(IPackageSourceConfiguration packageSourceConfiguration, IFileSystem fs, ILog log)
        {
            _packageSourceConfiguration = packageSourceConfiguration;
            _fs = fs;
            _log = log;
        }

        public bool SupportsPathType()
        {
            return _fs.Directory.Exists(_packageSourceConfiguration.PackageSource);
        }

        public PackageLocation<PackagePointer> CanFindPackage(string appName, SemanticVersion version = null)
        {
            try
            {
                var searchPath = _packageSourceConfiguration.PackageSource;
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

        public PackageLocation<object> CanFindPackageAsObject(string appName, SemanticVersion version = null)
        {
            var inner = CanFindPackage(appName);
            return inner == null ? null : new PackageLocation<object> { PackageDetails = inner.PackageDetails };
        }
    }
}