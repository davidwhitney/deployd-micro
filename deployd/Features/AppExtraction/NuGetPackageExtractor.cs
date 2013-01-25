using System;
using System.IO;
using NuGet;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class NuGetPackageExtractor : IPackageExtractor
    {
        private readonly ILog _log;

        public NuGetPackageExtractor(ILog log)
        {
            _log = log;
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

            var nugetPackage = packageInfo as IPackage;
            var files = nugetPackage.GetFiles();
            
            foreach (var file in files)
            {
                var fileOutputPath = Path.Combine(targetDirectory, file.Path);

                _log.DebugFormat("Writing file {0} to {1}...", file.Path, fileOutputPath);
               
                var directoryPath = Path.GetDirectoryName(fileOutputPath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllBytes(fileOutputPath, file.GetStream().ReadAllBytes());
            }
        }
    }
}