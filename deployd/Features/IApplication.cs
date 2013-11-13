using System;
using System.IO;

namespace deployd.Features
{
    public interface IApplication
    {
        bool IsInstalled { get; }
        bool IsStaged { get; }
        void BackupAndInstall();
        void ActivateStaging();
        void BackupCurrentVersion();
        void WriteUpdatedManifest(string newVersion);
        void PruneBackups();
        Version GetInstalledVersion();
        Version GetLatestAvailableVersion();
        Version GetStagedVersion();
    }
}