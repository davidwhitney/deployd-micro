using System;
using System.IO;
using NuGet;
using deployd.Extensibility.Configuration;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class NuGetPackageExtractor : IPackageExtractor
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly ILog _log;
        private readonly IPackageCache _packageCache;
        private readonly TextWriter _output;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly IApplicationMap _applicationMap;

        public NuGetPackageExtractor(System.IO.Abstractions.IFileSystem fs, ILog log, IPackageCache packageCache,
            TextWriter output, IInstanceConfiguration instanceConfiguration, IApplicationMap applicationMap)
        {
            _fs = fs;
            _log = log;
            _packageCache = packageCache;
            _output = output;
            _instanceConfiguration = instanceConfiguration;
            _applicationMap = applicationMap;
        }

        public bool CanUnpack(object packageInfo)
        {
            return packageInfo is IPackage;
        }

        public void Unpack(string targetDirectory, object packageInfo)
        {
            if (!(packageInfo is IPackage))
            {
                throw new InvalidOperationException("Somehow selected the wrong unpacker");
            }

            if (!UnpackIsRequired()
                 && !_instanceConfiguration.ForceUnpack)
            {
                _output.WriteLine("Skipping package unpacking as version {0} is already staged. Force unpack by specifying the -fu argument.",
                    _instanceConfiguration.Version);
                return;
            }

            _output.WriteLine("Unpacking NuGet package...");

            var nugetPackage = packageInfo as IPackage;
            nugetPackage = _packageCache.CachePackage(nugetPackage);

            var files = nugetPackage.GetFiles();
            
            foreach (var file in files)
            {
                var fileOutputPath = Path.Combine(targetDirectory, file.Path);

                _log.DebugFormat("Writing file {0} to {1}...", file.Path, fileOutputPath);
               
                var directoryPath = Path.GetDirectoryName(fileOutputPath);
                _fs.EnsureDirectoryExists(directoryPath);
                _fs.File.WriteAllBytes(fileOutputPath, file.GetStream().ReadAllBytes());
            }
        }

        private bool UnpackIsRequired()
        {
            string existingVersion = _fs.File.ReadAllText(_applicationMap.VersionFile);
            if (_instanceConfiguration.Version.Equals(existingVersion))
            {
                return false;
            }
            return true;
        }
    }
}