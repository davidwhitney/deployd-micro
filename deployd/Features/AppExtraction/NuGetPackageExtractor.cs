using System;
using System.IO;
using NuGet;

namespace deployd.Features.AppExtraction
{
    public class NuGetPackageExtractor : IPackageExtractor
    {
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
                
                //_logger.DebugFormat("Writing file {0} to {1}...", file.Path, fileOutputPath);
               
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