﻿using System;
using System.IO;
using NuGet;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class NuGetPackageExtractor : IPackageExtractor
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly ILog _log;

        public NuGetPackageExtractor(System.IO.Abstractions.IFileSystem fs, ILog log)
        {
            _fs = fs;
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

            _log.Info("Unpacking NuGet package...");

            var nugetPackage = packageInfo as IPackage;
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
    }
}