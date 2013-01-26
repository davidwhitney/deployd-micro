using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Features.ClientConfiguration;
using deployd.Infrastructure;
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
                            PackageDetails = new PackagePointer {PathAndFileName = latestPackage.ToAbsolutePath()}
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

    public class PackagePointer
    {
        public string PathAndFileName { get; set; }
    }
}