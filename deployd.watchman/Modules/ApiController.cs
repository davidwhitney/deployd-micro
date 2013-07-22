using Nancy;
using deployd.watchman.Services;
using log4net;

namespace deployd.watchman.Modules
{
    public class ApiController : NancyModule
    {
        private readonly AppService _appService;
        private readonly ConfigurationService _configurationService;
        private ILog _logger = null;
        
        private static string ApiRoot
        {
            get { return "/api/v1"; }
        }

        public ApiController(AppService appService, ConfigurationService configurationService) 
            : base(ApiRoot)
        {
            _logger = LogManager.GetLogger(typeof(ApiController));
            
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

            Post["/install-queue/{AppName}"] = x =>
                {
                    string environment = Request.Query["environment"];
                    bool prepareOnly = Request.Query["prepareOnly"] != null;
                    bool forceDownload = Request.Query["forceDownload"] != null;
                    _logger.DebugFormat("Install {0} ({1})", x.AppName, environment);
                    _appService.InstallPackage((string)x.AppName, prepareOnly, forceDownload, environment);
                    var response = new {next = ApiRoot + "/apps/" + x.AppName};
                    return Response.AsJson(response, HttpStatusCode.Created);
                };
        }
    }
}