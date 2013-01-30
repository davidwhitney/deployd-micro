using Nancy;
using deployd.watchman.Services;

namespace deployd.watchman.Modules
{
    public class ApiController : NancyModule
    {
        private readonly AppService _appService;
        private readonly ConfigurationService _configurationService;

        public ApiController(AppService appService, ConfigurationService configurationService) 
            : base("/api/v1")
        {
            _appService = appService;
            _configurationService = configurationService;

            Get["/apps"] = x =>
                {
                    var apps = _appService.InitedApps();
                    var installPath = _configurationService.DeploymentPath();
                    return Response.AsJson(new { installPath, total = apps.Count, apps });
                };

            Get["/apps/{AppName}"] = x =>
                {
                    var appName = (string) x.AppName;
                    var backupVersions = _appService.AvailableBackupVersions(appName);
                    var version = _appService.InstalledVersion(appName);
                    return Response.AsJson(new {appName, version, backupVersions});
                };

            Put["/apps/{AppName}"] = x =>
                {
                    _appService.InstallPackage((string)x.AppName);
                    return Response.AsJson("ok", HttpStatusCode.Created);
                };
        }
    }
}