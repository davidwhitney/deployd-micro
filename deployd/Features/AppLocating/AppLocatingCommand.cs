using System.Collections.Generic;
using System.Linq;
using deployd.Features.ClientConfiguration;
using deployd.Features.FeatureSelection;

namespace deployd.Features.AppLocating
{
    public class AppLocatingCommand : IFeatureCommand
    {
        private readonly List<IAppInstallationLocator> _finders;
        
        public Configuration Configuration { get; set; }
        public InstanceConfiguration InstanceConfiguration { get; set; }

        public AppLocatingCommand(IEnumerable<IAppInstallationLocator> finders)
        {
            _finders = finders.ToList();
        }

        public void Execute()
        {
            var location =
                _finders.Select(locator => locator.CanFindPackageAsObject(InstanceConfiguration.AppName))
                        .FirstOrDefault(result => result != null);

            if (location != null)
            {
                InstanceConfiguration.PackageLocation = location;
            }
        }
    }
}
