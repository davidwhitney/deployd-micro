using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using deployd.Extensibility.Configuration;
using deployd.Features.AppInstallation;
using deployd.Infrastructure;
using log4net;

namespace deployd.Features
{
    public class Application : IApplication
    {
        private readonly IFileSystem _fs;
        private readonly IApplicationMap _appMap;
        private readonly ILog _log;
        private readonly IInstanceConfiguration _config;
        private readonly IInstallationPadLock _installationLock;

        public bool IsInstalled { get { return _fs.File.Exists(_appMap.VersionFile); } }
        public bool IsStaged { get { return _fs.Directory.Exists(_appMap.Staging); } }
        
        private const int TotalBackupsToKeep = 10;

        public Application(IApplicationMap appMap, IFileSystem fs, ILog log, IInstanceConfiguration config, IInstallationPadLock installationLock)
        {
            _fs = fs;
            _appMap = appMap;
            _log = log;
            _config = config;
            _installationLock = installationLock;

            _log.Info("App directory: " + _appMap.FullPath);
        }

        public void EnsureDataDirectoriesExist()
        {
            _fs.EnsureDirectoryExists(_appMap.FullPath);
            _fs.EnsureDirectoryExists(_appMap.Staging);
        }

        public void LockForInstall()
        {
            _installationLock.LockAppInstallation();            
        }

        public void UpdateToLatestRevision()
        {
            BackupCurrentVersion();
            ActivateStaging();
            WriteUpdatedManifest(_config.PackageLocation.PackageVersion);
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

            var currentInstalledVersion = GetInstalledVersion();
            var backupPath = Path.Combine(_appMap.FullPath, currentInstalledVersion.ToString());

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

        public void PruneBackups()
        {
            var backups = _fs.Directory.GetDirectories(_appMap.FullPath);
            if (backups.Length <= 10)
            {
                return;
            }

            var oldestFirst = backups.Reverse().ToArray();
            var itemsToRemove = oldestFirst.Skip(TotalBackupsToKeep).ToList();

            foreach (var item in itemsToRemove)
            {
                _fs.Directory.Delete(item, true);
            }
        }

        public Version GetInstalledVersion()
        {
            return new Version(_fs.File.ReadAllText(_appMap.VersionFile));
        }

        public Version GetLatestAvailableVersion()
        {
            throw new NotImplementedException();
        }
    }
}