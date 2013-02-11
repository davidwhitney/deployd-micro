using System;
using System.IO;
using System.IO.Abstractions;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features
{
    public class Application
    {
        private readonly IFileSystem _fs;
        private readonly IApplicationMap _appMap;
        private readonly ILog _log;

        public bool IsInstalled { get { return _fs.File.Exists(_appMap.VersionFile); } }
        public bool IsStaged { get { return _fs.Directory.Exists(_appMap.Staging); } }

        public Application(IApplicationMap appMap, IFileSystem fs, ILog log)
        {
            _fs = fs;
            _appMap = appMap;
            _log = log;
        }

        public void UpdateToVersion(string version)
        {
            BackupCurrentVersion();
            ActivateStaging();
            WriteUpdatedManifest(version);
        }

        public void ActivateStaging()
        {
            _log.Info("Activating staged install...");
            _fs.Directory.Move(_appMap.Staging, _appMap.Active);
        }

        public void BackupCurrentVersion()
        {
            if (!IsInstalled)
            {
                // No version file? No previous install!
                return;
            }

            var currentInstalledVersion = _fs.File.ReadAllText(_appMap.VersionFile);
            var backupPath = Path.Combine(_appMap.FullPath, currentInstalledVersion);

            if (_fs.Directory.Exists(backupPath))
            {
                var newPath = backupPath + "-duplicate-" + Guid.NewGuid().ToString();
                _fs.Directory.Move(backupPath, newPath);
            }

            if (_fs.Directory.Exists(_appMap.Active))
            {
                _log.Info("Backing up current installation...");
                _fs.Directory.Move(_appMap.Active, backupPath);
            }
        }

        public void WriteUpdatedManifest(string newVersion)
        {
            _fs.File.WriteAllText(_appMap.VersionFile, newVersion);
        }
    }
}