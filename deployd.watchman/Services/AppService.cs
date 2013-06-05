using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;

namespace deployd.watchman.Services
{
    public class AppService
    {
        private readonly ConfigurationService _cfgService;
        private readonly IFileSystem _fs;

        public AppService(ConfigurationService cfgService)
            : this(cfgService, new FileSystem())
        {
        }

        private AppService(ConfigurationService cfgService, IFileSystem fs)
        {
            _cfgService = cfgService;
            _fs = fs;
        }

        public List<string> InitedApps()
        {
            if (!_fs.Directory.Exists(_cfgService.DeploymentPath()))
            {
                return new List<string>();
            }

            return
                _fs.Directory.GetDirectories(_cfgService.DeploymentPath())
                   .Select(y => y.Replace(_cfgService.DeploymentPath() + "\\", ""))
                   .ToList();
        } 

        public List<string> AvailableBackupVersions(string appName)
        {
            var map = _cfgService.AppMapFor(appName);

            if (!_fs.Directory.Exists(map.FullPath))
            {
                return new List<string>();
            }

            return _fs.Directory.GetDirectories(map.FullPath)
                      .Select(y => y.Replace(map.FullPath + "\\", ""))
                      .Where(y => y != "Active")
                      .ToList();
        } 

        public string InstalledVersion(string appName)
        {
            var map = _cfgService.AppMapFor(appName);
            return !_fs.File.Exists(map.VersionFile) ? string.Empty : _fs.File.ReadAllText(map.VersionFile);
        }

        public void InstallPackage(string appName, string environment="")
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "deployd.exe",
                Arguments = string.Format("-install -app=\"{0}\" -e=\"{1}\"", appName, environment),
            };

            Process.Start(startInfo);
        }
    }
}
