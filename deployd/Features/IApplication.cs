namespace deployd.Features
{
    public interface IApplication
    {
        bool IsInstalled { get; }
        bool IsStaged { get; }
        void UpdateToLatestRevision();
        void ActivateStaging();
        void BackupCurrentVersion();
        void WriteUpdatedManifest(string newVersion);
        void PruneBackups();
    }
}