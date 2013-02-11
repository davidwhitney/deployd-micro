namespace deployd.Features
{
    public interface IInstallationRoot
    {
        string Path { get; }
        void EnsureInstallationDirectoryExists();
    }
}