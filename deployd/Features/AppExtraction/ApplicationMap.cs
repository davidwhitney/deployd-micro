using System.IO;
using deployd.Infrastructure;

namespace deployd.Features.AppExtraction
{
    public class ApplicationMap
    {
        public string FullPath { get; set; }
        public string Staging { get; set; }
        public string Active { get; set; }
        public string Lockfile { get; set; }
        public string VersionFile { get; set; }

        public FileStream Lock { get; set; }

        public ApplicationMap(string fullPath)
        {
            FullPath = fullPath;
            Staging = Path.Combine(FullPath, "Staging").ToAbsolutePath();
            Active = Path.Combine(fullPath, "Active").ToAbsolutePath();
            Lockfile = Path.Combine(fullPath, "installing.deployd").ToAbsolutePath();
            VersionFile = Path.Combine(fullPath, "version.deployd").ToAbsolutePath();
        }

    }
}