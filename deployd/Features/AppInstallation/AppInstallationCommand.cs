using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using System;
using System.IO.Abstractions;

namespace deployd.Features.AppInstallation
{
    public class AppInstallationCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;
        private readonly InstallHookExecutor _hookExecutor;

        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppInstallationCommand(IFileSystem fs, Configuration configuration, InstallHookExecutor hookExecutor)
        {
            _fs = fs;
            _hookExecutor = hookExecutor;
            Configuration = configuration;
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
            _fs.Directory.Move(InstanceConfiguration.AppDirectory.Staging, InstanceConfiguration.AppDirectory.Active);
        }

        private void BackupPreviousInstallation()
        {
            if (_fs.Directory.Exists(InstanceConfiguration.AppDirectory.Backup))
            {
                _fs.Directory.Delete(InstanceConfiguration.AppDirectory.Backup, true);
            }

            if (_fs.Directory.Exists(InstanceConfiguration.AppDirectory.Active))
            {
                _fs.Directory.Move(InstanceConfiguration.AppDirectory.Active, InstanceConfiguration.AppDirectory.Backup);
            }
        }
    }
}