using System.IO.Abstractions;
using System.Linq;
using Nancy;
using deployd.watchman.Services;

namespace deployd.watchman.Modules
{
    public class ApiController : NancyModule
    {
        private readonly ConfigurationService _cfgService;
        private readonly InstallationService _installationService;
        private readonly IFileSystem _fs = new FileSystem();

        public ApiController(ConfigurationService cfgService, InstallationService installationService) 
            : base("/api/v1")
        {
            _cfgService = cfgService;
            _installationService = installationService;

            Get["/apps"] = x =>
                {
                    var initedApps =
                        _fs.Directory.GetDirectories(_cfgService.DeploymentPath())
                           .Select(y => y.Replace(_cfgService.DeploymentPath() + "\\", ""))
                           .ToList();

                    return Response.AsJson(new { total = initedApps.Count, apps = initedApps });
                };

            Get["/apps/{AppName}"] = x =>
                {
                    var map = _cfgService.AppMapFor((string) x.AppName);

                    var backups =
                        _fs.Directory.GetDirectories(map.FullPath)
                           .Select(y => y.Replace(map.FullPath + "\\", ""))
                           .Where(y => y != "Active")
                           .ToList();
                    
                    return Response.AsJson(new
                        {
                            appName = x.AppName,
                            version = _fs.File.ReadAllText(map.VersionFile),
                            backupVersions = backups
                        });
                };

            Put["/apps/{AppName}"] = x =>
                {
                    _installationService.InstallPackage((string)x.AppName);
                    return Response.AsJson("ok", HttpStatusCode.Created);
                };
        }
    }
}