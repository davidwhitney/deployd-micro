using System;
using Ionic.Zip;
using deployd.Features.AppLocating;
using log4net;

namespace deployd.Features.AppExtraction
{
    public class ZipFilePackageExtractor : IPackageExtractor
    {
        private readonly ILog _log;

        public ZipFilePackageExtractor(ILog log)
        {
            _log = log;
        }

        public bool CanUnpack(object packageInfo)
        {
            return packageInfo is PackagePointer && ((PackagePointer)packageInfo).IsZipFile;
        }

        public void Unpack(string targetDirectory, object packageInfo)
        {
            if (!(packageInfo is PackagePointer))
            {
                throw new InvalidOperationException("Somehow selected the wrong unpacker");
            }

            var pointer = (PackagePointer) packageInfo;

            _log.Info("Unpacking zip...");

            using (var zip = ZipFile.Read(pointer.PathAndFileName))
            {
                foreach (var e in zip)
                {
                    e.Extract(targetDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}