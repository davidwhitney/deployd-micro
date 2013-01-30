using System;
using System.Collections.Generic;
using System.Linq;
using deployd.Extensibility.Configuration;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;
using log4net;

namespace deployd.Features.AppLocating
{
    public class AppLocatingCommand : IFeatureCommand
    {
        private readonly ILog _log;
        private readonly List<IAppInstallationLocator> _finders;
        
        public DeploydConfiguration DeploydConfiguration { get; set; }
        public IInstanceConfiguration Config { get; set; }

        public AppLocatingCommand(IEnumerable<IAppInstallationLocator> finders, ILog log)
        {
            _log = log;
            _finders = finders.ToList();
        }

        public void Execute()
        {
            _log.Info("Serching for package: " + Config.AppName);

            var activeFinders = _finders.Where(x => x.SupportsPathType()).ToList();

            var location =
                activeFinders.Select(locator => locator.CanFindPackageAsObject(Config.AppName))
                             .FirstOrDefault(result => result != null);

            if (location != null)
            {
                _log.Info("Package found.");
                Config.PackageLocation = location;
                return;
            }
            
            _log.Info("No package matching " + Config.AppName + " found.");

            Environment.Exit(-1);
        }
    }
}
