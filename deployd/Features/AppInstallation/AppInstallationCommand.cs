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
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppInstallationCommand(IFileSystem fs, DeploydConfiguration deploydConfiguration, InstallHookExecutor hookExecutor, ILog log)
        {
            _fs = fs;
            _hookExecutor = hookExecutor;
            _log = log;
            DeploydConfiguration = deploydConfiguration;
        }

        public void Execute()
        {
            if (!_fs.Directory.Exists(InstanceConfiguration.AppDirectory.Staging))
            {
                throw new InvalidOperationException("Application isn't staged. Can't install.");
            }

            if (!_fs.Directory.Exists(InstanceConfiguration.AppDirectory.Active))
            {
                _hookExecutor.ExecuteFirstInstall();
            }

            _hookExecutor.ExecutePreInstall();

            BackupPreviousInstallation();
            MakeStagingActive();

            _hookExecutor.ExecutePostInstall();
        }

        private void MakeStagingActive()
        {
            _log.Info("Activating staged install...");
            _fs.Directory.Move(InstanceConfiguration.AppDirectory.Staging, InstanceConfiguration.AppDirectory.Active);
        }

        private void BackupPreviousInstallation()
        {
            if (_fs.Directory.Exists(InstanceConfiguration.AppDirectory.Backup))
            {
                _log.Info("Removing last backup...");
                _fs.Directory.Delete(InstanceConfiguration.AppDirectory.Backup, true);
            }

            if (_fs.Directory.Exists(InstanceConfiguration.AppDirectory.Active))
            {
                _log.Info("Backing up current installation...");
                _fs.Directory.Move(InstanceConfiguration.AppDirectory.Active, InstanceConfiguration.AppDirectory.Backup);
            }
        }
    }
}