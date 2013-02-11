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
            var app = new Application(_config.ApplicationMap, _fs, _log);

            if (!app.IsStaged)
            {
                throw new InvalidOperationException("Application isn't staged. Can't install.");
            }

            if (!app.IsInstalled)
            {
                _hookExecutor.ExecuteFirstInstall();
            }

            _hookExecutor.ExecutePreInstall();

            app.UpdateToVersion(_config.PackageLocation.PackageVersion);

            _hookExecutor.ExecutePostInstall();
        }
    }
}