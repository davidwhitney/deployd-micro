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

                byte[] fileData = null;

                using (var readStream = file.GetStream())
                {
                    try
                    {
                        fileData = readStream.ReadAllBytes();
                    }
                    finally
                    {
                        readStream.Close();
                    }
                }

                using (var writeStream = _fs.File.Open(fileOutputPath, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        writeStream.Write(fileData, 0, fileData.Length);
                        writeStream.Flush();
                    }
                    finally
                    {
                        writeStream.Close();
                    }
                }
            }
        }

        
    }
}