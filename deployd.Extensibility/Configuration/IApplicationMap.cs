using System.IO;

namespace deployd.Extensibility.Configuration
{
    public interface IApplicationMap
    {
        string AppName { get; set; }
        string InstallPath { get; set; }
        string Staging { get; set; }
        string Lockfile { get; set; }
        string VersionFile { get; set; }
        Stream Lock { get; set; }
        string CachePath { get; set; }
        IApplicationMap Configure(string appName, string installPath);
    }
}