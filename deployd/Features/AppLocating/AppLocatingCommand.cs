using System.Collections.Generic;
using System.IO;
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
        private readonly TextWriter _output;
        private readonly IEnumerable<IAppInstallationLocator> _finders;
        
        public AppLocatingCommand(IEnumerable<IAppInstallationLocator> finders, ILog log, IInstanceConfiguration config,
            TextWriter output)
        {
            _log = log;
            _config = config;
            _output = output;
            _finders = finders;
        }

        public void Execute()
        {
            _output.WriteLine("Searching for package {0} ({1})", _config.AppName, _config.Version ?? "latest");

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
                        _output.WriteLine("The specific version was not found, but the repository does have other version of the package. Latest available is {0}.", location.PackageVersion);
                        return;
                    }

                    _output.WriteLine("Package not found.");
                }
                
                return;
            }

            _output.WriteLine("Found {0} v{1}", location.PackageDetails, location.PackageVersion);
            _config.Version = location.PackageVersion;
            _config.PackageLocation = location;
        }
    }
}
