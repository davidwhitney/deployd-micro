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

            if (!_fs.Directory.Exists(map.InstallPath))
            {
                return new List<string>();
            }

            return _fs.Directory.GetDirectories(map.InstallPath)
                      .Select(y => y.Replace(map.InstallPath + "\\", ""))
                      .Where(y => y != "Active")
                      .ToList();
        } 

        public string InstalledVersion(string appName)
        {
            var map = _cfgService.AppMapFor(appName);
            return !_fs.File.Exists(map.VersionFile) ? string.Empty : _fs.File.ReadAllText(map.VersionFile);
        }

        public void InstallPackage(string appName, bool prepareOnly, bool forceDownload, string environment)
        {
            _log.InfoFormat("Installing {0} ({1}, {2}, {3})", appName, environment, prepareOnly ? "prepare" : "install", forceDownload ? "force download" : "use cached if available");
            var process = new Process();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.StartInfo.FileName = "deployd.exe";
            process.StartInfo.Arguments = string.Format("-{2} -app=\"{0}\" -e=\"{1}\" {3}", 
                appName, 
                environment,
                prepareOnly ? "p" : "i",
                forceDownload ? "-f" : "");
            process.OutputDataReceived += (sender, args) => _log.Info(args.Data);
            process.ErrorDataReceived += (sender, args) => _log.Warn(args.Data);
            process.Exited += (sender, args) => _log.DebugFormat("Process exited with code {0}", process.ExitCode);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            //p.WaitForExit();
        }
    }
}
