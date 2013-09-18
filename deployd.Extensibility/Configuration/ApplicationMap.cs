using System;
using System.IO;

namespace deployd.Extensibility.Configuration
{
    public class ApplicationMap : IApplicationMap
    {
        public string AppName { get; set; }
        public string InstallPath { get; set; }
        public string Staging { get; set; }
        public string Lockfile { get; set; }
        public string VersionFile { get; set; }
        public string CachePath { get; set; }
        public string StagingVersionFile { get; set; }

        public Stream Lock { get; set; }

        public ApplicationMap(string appName, string installPath)
        {
            Configure(appName, installPath);
        }

        public IApplicationMap Configure(string appName, string installPath)
        {
            AppName = appName;
            InstallPath = installPath.ToAbsolutePath();
            CachePath = Path.Combine(installPath, ".deployd").ToAbsolutePath();
            Staging = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,appName), "Staging").ToAbsolutePath();
            Staging = Path.Combine(CachePath, "Staging").ToAbsolutePath();
            //Active = Path.Combine(fullPath, "Active").ToAbsolutePath();
            Lockfile = Path.Combine(CachePath, "installing.deployd").ToAbsolutePath();
            VersionFile = Path.Combine(CachePath, "version.deployd").ToAbsolutePath();
            StagingVersionFile = Path.Combine(CachePath, "staging.version.deployd").ToAbsolutePath();
            return this;
        }
    }
}