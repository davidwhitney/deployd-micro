using System.Collections.Generic;
using System.Linq;
using NuGet;
using deployd.Extensibility.Configuration;
using log4net;

namespace deployd.Features.AppLocating
{
    public class AppLocatingCommand : IFeatureCommand
    {
        private readonly ILog _log;
        private readonly IInstanceConfiguration _config;
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        
        public AppLocatingCommand(IEnumerable<IAppInstallationLocator> finders, ILog log, IInstanceConfiguration config)
        {
            _log = log;
            _config = config;
            _finders = finders;
        }

        public void Execute()
        {
            _log.InfoFormat("Searching for package {0} ({1})", _config.AppName, _config.Version ?? "latest");

            var activeFinders = _finders.Where(x => x.SupportsPathType()).ToList();

            var location =
                activeFinders.Select(locator => locator.CanFindPackageAsObject(_config.AppName, _config.Version as SemanticVersion))
                             .FirstOrDefault(result => result != null);

            if (location == null)
            {
                if ((_config.Version as SemanticVersion) != null)
                {
                    location =
                            activeFinders.Select(locator => locator.CanFindPackageAsObject(_config.AppName, null))
                             .FirstOrDefault(result => result != null);
                    if (location != null)
                    {
                        _log.InfoFormat("The specific version was not found, but the repository does have other version of the package. Latest available is {0}.", location.PackageVersion);
                        return;
                    } 
                    
                    _log.Info("Package not found.");
                }
                
                return;
            }

            _log.Info("Package found.");
            _config.PackageLocation = location;
        }
    }
}
