using System.Collections.Generic;
using Nancy;
using deployd.watchman.Services;

namespace deployd.watchman.Modules
{
    public class HomeController : NancyModule
    {
        public HomeController(AppService appService, ConfigurationService configurationService)
        {
            Get["/"] = x => Response.AsText("deployd watchman");
            Get["/apps"] = x =>
                {
                    var apps = appService.InitedApps();
                    var installPath = configurationService.DeploymentPath();
                    var viewModel = new AppsViewModel();
                    viewModel.Apps = apps;
                    viewModel.InstallPath = installPath;
                    return Negotiate.WithModel(viewModel).WithView("apps.cshtml");
                };
            Get["/apps/{appName}"] = x =>
                {
                    var appName = (string)x.AppName;
                    var backupVersions = appService.AvailableBackupVersions(appName);
                    var version = appService.InstalledVersion(appName);
                    var viewModel = new AppViewModel();
                    viewModel.AppName = appName;
                    viewModel.BackupVersions = backupVersions;
                    viewModel.Version = version;
                    return Negotiate.WithModel(viewModel).WithView("app.cshtml");
                };
        }
    }

    public class AppViewModel
    {
        public string AppName { get; set; }

        public List<string> BackupVersions { get; set; }

        public string Version { get; set; }
    }

    public class AppsViewModel
    {
        public List<string> Apps { get; set; }

        public string InstallPath { get; set; }
    }
}
