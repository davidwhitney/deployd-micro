using System.IO;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
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

        public DeploydConfiguration DeploydConfiguration { get; set; }
        public InstanceConfiguration Config { get; set; }

        public AppInstallationCommand(IFileSystem fs, DeploydConfiguration deploydConfiguration, InstallHookExecutor hookExecutor, ILog log)
        {
            _fs = fs;
            _hookExecutor = hookExecutor;
            _log = log;
            DeploydConfiguration = deploydConfiguration;
        }

        public void Execute()
        {
            if (!_fs.Directory.Exists(Config.DirectoryMaps.Staging))
            {
                throw new InvalidOperationException("Application isn't staged. Can't install.");
            }

            if (!_fs.Directory.Exists(Config.DirectoryMaps.Active))
            {
                _hookExecutor.ExecuteFirstInstall();
            }

            _hookExecutor.ExecutePreInstall();

            BackupPreviousInstallation();
            MakeStagingActive();

            _fs.File.WriteAllText(
                Config.DirectoryMaps.VersionFile,
                Config.PackageLocation.PackageVersion);

            _hookExecutor.ExecutePostInstall();
        }

        private void MakeStagingActive()
        {
            _log.Info("Activating staged install...");
            _fs.Directory.Move(Config.DirectoryMaps.Staging, Config.DirectoryMaps.Active);
        }

        private void BackupPreviousInstallation()
        {
            if (!_fs.File.Exists(Config.DirectoryMaps.VersionFile))
            {
                // No version file? No previous install!
                return;
            }

            var currentInstalledVersion = _fs.File.ReadAllText(Config.DirectoryMaps.VersionFile);
            var backupPath = Path.Combine(Config.DirectoryMaps.FullPath, currentInstalledVersion);

            if (_fs.Directory.Exists(backupPath))
            {
                var newPath = backupPath + "-duplicate-" + Guid.NewGuid().ToString();
                _fs.Directory.Move(backupPath, newPath);
            }

            if (_fs.Directory.Exists(Config.DirectoryMaps.Active))
            {
                _log.Info("Backing up current installation...");
                _fs.Directory.Move(Config.DirectoryMaps.Active, backupPath);
            }
        }
    }
}