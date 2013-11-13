using System.IO.Abstractions;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;
using deployd.Infrastructure;

namespace deployd.Features
{
    public class InstallationRoot : IInstallationRoot
    {
        private readonly DeploydConfiguration _deploydConfiguration;
        private readonly IInstanceConfiguration _instanceConfiguration;
        private readonly IFileSystem _fs;
        private readonly string _absolutePath;
        private string _installPath=null;

        public string Path
        {
            get
            {
                if (_installPath == null)
                {
                    if (_instanceConfiguration.InstallPath != null
                        && _fs.Path.IsPathRooted(_instanceConfiguration.InstallPath))
                    {
                        _installPath = _instanceConfiguration.InstallPath;
                    }
                    else
                    {
                        _installPath = _fs.Path.Combine(_deploydConfiguration.InstallRoot, _instanceConfiguration.InstallPath ?? "");
                    }
                }

                return _installPath;
            }
        }

        public InstallationRoot(DeploydConfiguration deploydConfiguration, 
            IInstanceConfiguration instanceConfiguration,
            IFileSystem fs)
        {
            _deploydConfiguration = deploydConfiguration;
            _instanceConfiguration = instanceConfiguration;
            _fs = fs;
            
            _absolutePath = Path.ToAbsolutePath();
        }

        public void EnsureInstallationDirectoryExists()
        {
            _fs.EnsureDirectoryExists(_absolutePath);
        }
    }
}