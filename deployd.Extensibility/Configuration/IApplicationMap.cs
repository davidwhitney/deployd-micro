using System.IO;

namespace deployd.Extensibility.Configuration
{
    public interface IApplicationMap
    {
        string AppName { get; set; }
        string FullPath { get; set; }
        string Staging { get; set; }
        string Active { get; set; }
        string Lockfile { get; set; }
        string VersionFile { get; set; }
        FileStream Lock { get; set; }
        IApplicationMap For(string appName, string fullPath);
    }
}