using System.IO.Abstractions;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Infrastructure;

namespace deployd.Features
{
    public class InstallationRoot : IInstallationRoot
    {
        private readonly IFileSystem _fs;
        private readonly string _absolutePath;

        public string Path { get; private set; }

        public InstallationRoot(DeploydConfiguration deploydConfiguration, 
            IInstanceConfiguration instanceConfiguration,
            IFileSystem fs)
        {
            _fs = fs;
            Path = _fs.Path.Combine(deploydConfiguration.InstallRoot,instanceConfiguration.InstallationSubPath);
            _absolutePath = Path.ToAbsolutePath();
        }

        public void EnsureInstallationDirectoryExists()
        {
            _fs.EnsureDirectoryExists(_absolutePath);
        }
    }
}