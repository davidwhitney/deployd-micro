using System.Collections.Generic;
using System.Linq;
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
            _log.Info("Searching for package: " + _config.AppName);

            var activeFinders = _finders.Where(x => x.SupportsPathType()).ToList();

            var location =
                activeFinders.Select(locator => locator.CanFindPackageAsObject(_config.AppName))
                             .FirstOrDefault(result => result != null);

            if (location == null)
            {
                return;
            }

            _log.Info("Package found.");
            _config.PackageLocation = location;
        }
    }
}
