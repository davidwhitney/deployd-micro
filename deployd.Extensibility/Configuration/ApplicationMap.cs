using System.IO;

namespace deployd.Extensibility.Configuration
{
    public class ApplicationMap : IApplicationMap
    {
        public string AppName { get; set; }
        public string FullPath { get; set; }
        public string Staging { get; set; }
        public string Active { get; set; }
        public string Lockfile { get; set; }
        public string VersionFile { get; set; }

        public FileStream Lock { get; set; }

        public ApplicationMap(string appName, string fullPath)
        {
            AppName = appName;
            FullPath = fullPath;
            Staging = Path.Combine(FullPath, "Staging").ToAbsolutePath();
            Active = Path.Combine(fullPath, "Active").ToAbsolutePath();
            Lockfile = Path.Combine(fullPath, "installing.deployd").ToAbsolutePath();
            VersionFile = Path.Combine(fullPath, "version.deployd").ToAbsolutePath();
        }

    }
}