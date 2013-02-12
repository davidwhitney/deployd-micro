using System;

namespace deployd.Features.AppInstallation
{
    public class AppInstallationCommand : IFeatureCommand
    {
        private readonly IApplication _app;
        private readonly IInstallHookExecutor _hookExecutor;

        public AppInstallationCommand(IApplication app, IInstallHookExecutor hookExecutor)
        {
            _app = app;
            _hookExecutor = hookExecutor;
        }

        public void Execute()
        {
            if (!_app.IsStaged)
            {
                throw new InvalidOperationException("Application isn't staged. Can't install.");
            }

            if (!_app.IsInstalled)
            {
                _hookExecutor.ExecuteFirstInstall();
            }

            _hookExecutor.ExecutePreInstall();
            _app.UpdateToLatestRevision();
            _hookExecutor.ExecutePostInstall();
        }
    }
}