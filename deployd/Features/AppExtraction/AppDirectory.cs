using System.IO;
using deployd.Infrastructure;

namespace deployd.Features.AppExtraction
{
    public class AppDirectory
    {
        public string FullPath { get; set; }
        public string Staging { get; set; }
        public string Backup { get; set; }
        public string Active { get; set; }
        public string Lockfile { get; set; }

        public FileStream Lock { get; set; }

        public AppDirectory(string fullPath)
        {
            FullPath = fullPath;
            Staging = Path.Combine(FullPath, ".staging").ToAbsolutePath();
            Backup = Path.Combine(fullPath, ".previous").ToAbsolutePath();
            Active = Path.Combine(fullPath, ".active").ToAbsolutePath();
            Lockfile = Path.Combine(fullPath, "lock.deployd").ToAbsolutePath();
        }
    }
}