using System;
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
        public string CachePath { get; set; }

        public Stream Lock { get; set; }

        public ApplicationMap(string appName, string fullPath)
        {
            Configure(appName, fullPath);
        }

        public IApplicationMap Configure(string appName, string fullPath)
        {
            AppName = appName;
            FullPath = fullPath;
            CachePath = Path.Combine(fullPath, ".deployd").ToAbsolutePath();
            Staging = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,appName), "Staging").ToAbsolutePath();
            Staging = Path.Combine(CachePath, "Staging").ToAbsolutePath();
            //Active = Path.Combine(fullPath, "Active").ToAbsolutePath();
            Active = fullPath.ToAbsolutePath();
            Lockfile = Path.Combine(CachePath, "installing.deployd").ToAbsolutePath();
            VersionFile = Path.Combine(CachePath, "version.deployd").ToAbsolutePath();
            return this;
        }
    }
}