using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using log4net;

namespace deployd.watchman.Services
{
    public class AppService
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(AppService));
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
            _log.InfoFormat("Installing {0}", appName);
            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "deployd.exe";
            p.StartInfo.Arguments = string.Format("-install -app=\"{0}\" -e=\"{1}\"", appName, environment);

            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            _log.Debug(output);
        }
    }
}
