using System.IO;
using deployd.Extensibility.Configuration;
using System;
using System.IO.Abstractions;
using log4net;

namespace deployd.Features.AppInstallation
{
    public class AppInstallationCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly InstallHookExecutor _hookExecutor;
        private readonly ILog _log;
        private readonly IInstanceConfiguration _config;

        public AppInstallationCommand(IFileSystem fs, InstallHookExecutor hookExecutor, ILog log, IInstanceConfiguration config)
        {
            _fs = fs;
            _hookExecutor = hookExecutor;
            _log = log;
            _config = config;
        }

        public void Execute()
        {
            if (!_fs.Directory.Exists(_config.ApplicationMap.Staging))
            {
                throw new InvalidOperationException("Application isn't staged. Can't install.");
            }

            if (!_fs.File.Exists(_config.ApplicationMap.VersionFile))
            {
                _hookExecutor.ExecuteFirstInstall();
            }

            _hookExecutor.ExecutePreInstall();

            BackupPreviousInstallation();
            MakeStagingActive();

            _fs.File.WriteAllText(
                _config.ApplicationMap.VersionFile,
                _config.PackageLocation.PackageVersion);

            _hookExecutor.ExecutePostInstall();
        }

        private void MakeStagingActive()
        {
            _log.Info("Activating staged install...");
            _fs.Directory.Move(_config.ApplicationMap.Staging, _config.ApplicationMap.Active);
        }

        private void BackupPreviousInstallation()
        {
            if (!_fs.File.Exists(_config.ApplicationMap.VersionFile))
            {
                // No version file? No previous install!
                return;
            }

            var currentInstalledVersion = _fs.File.ReadAllText(_config.ApplicationMap.VersionFile);
            var backupPath = Path.Combine(_config.ApplicationMap.FullPath, currentInstalledVersion);

            if (_fs.Directory.Exists(backupPath))
            {
                var newPath = backupPath + "-duplicate-" + Guid.NewGuid().ToString();
                _fs.Directory.Move(backupPath, newPath);
            }

            if (_fs.Directory.Exists(_config.ApplicationMap.Active))
            {
                _log.Info("Backing up current installation...");
                _fs.Directory.Move(_config.ApplicationMap.Active, backupPath);
            }
        }
    }
}