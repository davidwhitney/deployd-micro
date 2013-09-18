using System.IO;
using System.IO.Abstractions;
using deployd.Extensibility;
using deployd.Extensibility.Configuration;

namespace deployd.watchman.Services
{
    public class ConfigurationService
    {
        private readonly DeploydConfigurationManager _cfgManager;

        public ConfigurationService()
        {
            _cfgManager = new DeploydConfigurationManager(new FileSystem(), new ApplicationFolderLocator());
        }

        public DeploydConfiguration LoadConfiguration()
        {
            return _cfgManager.LoadConfig();
        }

        public string DeploymentPath()
        {
            return LoadConfiguration().InstallRoot.ToAbsolutePath();
        }

        public IApplicationMap AppMapFor(string appName)
        {
            var appPath = Path.Combine(DeploymentPath(), appName);
            return new ApplicationMap(appName, appPath);
        }
    }
}
