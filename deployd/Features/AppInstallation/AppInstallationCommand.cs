using System;
using System.IO;
using System.IO.Abstractions;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using deployd.Infrastructure;

namespace deployd.Features.AppInstallation
{
    public class AppInstallationCommand : IFeatureCommand
    {
        private readonly IFileSystem _fs;

        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppInstallationCommand(IFileSystem fs, Configuration configuration)
        {
            _fs = fs;
            Configuration = configuration;
        }

        public void Execute()
        {
            var appDirectory = Path.Combine(Configuration.InstallRoot, InstanceConfiguration.AppName).ToAbsolutePath();
            var installationStaging = Path.Combine(appDirectory, ".staging").ToAbsolutePath();
            var installationBackup = Path.Combine(appDirectory, ".previous").ToAbsolutePath();
            var installationActive = Path.Combine(appDirectory, ".active").ToAbsolutePath();

            if (!_fs.Directory.Exists(installationStaging))
            {
                throw new InvalidOperationException("Application isn't staged. Can't install.");
            }

            BackupPreviousInstallation(installationBackup, installationActive);
            MakeStagingActive(installationStaging, installationActive);
        }

        private void MakeStagingActive(string installationStaging, string installationActive)
        {
            _fs.Directory.Move(installationStaging, installationActive);
        }

        private void BackupPreviousInstallation(string installationBackup, string installationActive)
        {
            if (_fs.Directory.Exists(installationBackup))
            {
                _fs.Directory.Delete(installationBackup);
            }

            if (_fs.Directory.Exists(installationActive))
            {
                _fs.Directory.Move(installationActive, installationBackup);
            }
        }
    }
}
