using System;
using Ionic.Zip;
using deployd.Features.AppLocating;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class ZipFilePackageExtractor : IPackageExtractor
    {
        private readonly System.IO.Abstractions.IFileSystem _fs;
        private readonly ILog _log;

        public ZipFilePackageExtractor(System.IO.Abstractions.IFileSystem fs, ILog log)
        {
            _fs = fs;
            _log = log;
        }

        public bool CanUnpack(object packageInfo)
        {
            return packageInfo is PackagePointer
                && ((PackagePointer)packageInfo).PathAndFileName.ToLower().EndsWith(".zip");
        }

        public void Unpack(string targetDirectory, object packageInfo)
        {
            if (!(packageInfo is PackagePointer))
            {
                throw new InvalidOperationException("Somehow selected the wrong unpacker");
            }

            var pointer = (PackagePointer) packageInfo;

            using (var zip = ZipFile.Read(pointer.PathAndFileName))
            {
                foreach (var e in zip)
                {
                    _log.DebugFormat("Writing file {0}...", e.FileName); 
                    e.Extract(targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}